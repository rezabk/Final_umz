using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.StudentPracticeService;
using Application.ViewModels.Practice;
using Application.ViewModels.PracticeQuestion;
using Common;
using Common.Enums;
using Common.ExceptionType.CustomException;
using Domain.Entities.PracticeEntities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.StudentPracticeService;

public class StudentPracticeService : ServiceBase<StudentPracticeService>, IStudentPracticeService
{
    private readonly IRepository<Practice> _practiceRepository;
    private readonly IRepository<PracticeQuestion> _practiceQuestionRepository;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IUploader _uploader;
    private readonly ICustomLoggerService<StudentPracticeService> _logger;

    public StudentPracticeService(IUnitOfWork unitOfWork, IConfiguration configuration, IUploader uploader,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment hostingEnvironment,
        ICustomLoggerService<StudentPracticeService> logger) : base(httpContextAccessor)
    {
        _practiceRepository = unitOfWork.GetRepository<Practice>();
        _practiceQuestionRepository = unitOfWork.GetRepository<PracticeQuestion>();
        _configuration = configuration;
        _hostingEnvironment = hostingEnvironment;
        _uploader = uploader;
        _logger = logger;
    }

    public Task<List<ShowPracticeByClassId>> GetAllPracticeByClassId(int classId)
    {
        var practices = _practiceRepository.DeferredWhere(x => x.Class != null && x.ClassId == classId)
            .Include(x => x.PracticeQuestions);

        return Task.FromResult(practices.Select(x => new ShowPracticeByClassId
        {
            Id = x.Id,
            Description = x.Description,
            ClassId = x.ClassId,
            ClassTitle = x.Class.Title,
            EndDate = x.EndDate.ConvertMiladiToJalali(),
            StartDate = x.StartDate.ConvertMiladiToJalali(),
            PracticeQuestionsCount = x.PracticeQuestions.Count
        }).ToList());
    }

    public Task<List<ShowPracticeQuestionViewModel>> GetAllPracticeQuestionByPracticeId(int practiceId)
    {
        var practiceQuestions =
            _practiceQuestionRepository.DeferredWhere(x => x.Practice != null && x.PracticeId == practiceId);


        return Task.FromResult(practiceQuestions.Select(x => new ShowPracticeQuestionViewModel
        {
            Id = x.Id,
            Description = x.Description,
            FileName = x.FileName,
            Title = x.Title
        }).ToList());
    }

    public Task<bool> AnswerPracticeQuestion(RequestAnswerPracticeQuestionViewModel model)
    {
        var practiceQuestion =
            _practiceQuestionRepository.DeferredWhere(x => x.Practice != null && x.Id == model.PracticeQuestionId)
                .Include(x => x.UserAnsweredQuestions).Include(x => x.PracticeQuestionAnswers).Include(x=>x.Practice)
                .FirstOrDefault() ?? throw new NotFoundException();

        if (practiceQuestion.UserAnsweredQuestions.Select(x => x.UserId).Contains(CurrentUserId))
            throw new FormValidationException(MessageId.AlreadyAnsweredQuestion);

        if (DateTime.Now > practiceQuestion.Practice.EndDate)
            throw new FormValidationException(MessageId.DeadlineReached);

        var newPracticeQuestionAnswer = new PracticeQuestionAnswer
            { PracticeQuestionId = model.PracticeQuestionId, Description = model.Description, UserId = CurrentUserId };

        if (model.File != null)
        {
            var filePath = UploadFile(model.File);
            newPracticeQuestionAnswer.FileName = string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
            newPracticeQuestionAnswer.FileExtension =
                string.IsNullOrEmpty(filePath) ? null : Path.GetExtension(filePath);
        }


        try
        {
            practiceQuestion.PracticeQuestionAnswers.Add(newPracticeQuestionAnswer);
            practiceQuestion.UserAnsweredQuestions.Add(new UserAnsweredQuestion
                { PracticeQuestionId = practiceQuestion.Id, UserId = CurrentUserId });

            _practiceQuestionRepository.UpdateAsync(practiceQuestion, true);
            _logger.LogUpdateSuccess("PracticeQuestion", practiceQuestion.Id);
            return Task.FromResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogUpdateError(exception, "PracticeQuestion", practiceQuestion.Id);
            throw exception ?? throw new ErrorException();
        }
    }

    public Task<string> GetQuestionImage(string fileName)
    {
        var practice =
            _practiceQuestionRepository.DeferredWhere(x => x.Practice != null && x.FileName == fileName)
                .FirstOrDefault() ?? throw new NotFoundException();

        var folderPath = _hostingEnvironment.ContentRootPath +
                         _configuration.GetSection("File:ImageFilePracticeQuestions").Value;
        var fullPath = Path.Combine(folderPath, practice.FileName);

        return Task.FromResult(fullPath);
    }

    private string UploadFile(IFormFile file)
    {
        var fullFilePath = _uploader.UploadFile(file,
            _hostingEnvironment.ContentRootPath + _configuration.GetSection("File:FilePracticeQuestionAnswers").Value,
            "").Result;

        return fullFilePath;
    }
}