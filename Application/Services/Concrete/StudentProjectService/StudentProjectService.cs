using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.StudentProjectService;
using Application.ViewModels.Project;
using Application.ViewModels.Public;
using Common;
using Common.Enums;
using Common.ExceptionType.CustomException;
using Domain.Entities.ProjectEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.StudentProjectService;

public class StudentProjectService : ServiceBase<StudentProjectService>, IStudentProjectService
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<ProjectAnswer> _projectAnswerRepository;
    private readonly IConfiguration _configuration;
    private readonly ILiaraUploader _uploader;
    private readonly ICustomLoggerService<StudentProjectService> _logger;


    public StudentProjectService(IUnitOfWork unitOfWork, IConfiguration configuration, ILiaraUploader uploader,
        IHttpContextAccessor httpContextAccessor,
        ICustomLoggerService<StudentProjectService> logger) : base(httpContextAccessor)
    {
        _projectRepository = unitOfWork.GetRepository<Project>();
        _projectAnswerRepository = unitOfWork.GetRepository<ProjectAnswer>();
        _configuration = configuration;
        _uploader = uploader;
        _logger = logger;
    }

    public Task<List<ShowProjectViewModel>> GetAllProjectByClassId(int classId)
    {
        var projects = _projectRepository.DeferredWhere(x => x.ClassId == classId && x.Class.Students.Select(x => x.Id)
                .Contains(CurrentUserId)).OrderByDescending(x => x.Id)
            .Include(x => x.Class);

        return Task.FromResult(projects.Select(x => new ShowProjectViewModel
        {
            Id = x.Id,
            Description = x.Description,
            ClassId = x.ClassId,
            ClassTitle = x.Class.Title,
            EndDate = x.EndDate.ConvertMiladiToJalali(),
            StartDate = x.StartDate.ConvertMiladiToJalali(),
            Title = x.Title,
            FileName = x.FileName
        }).ToList());
    }

    public Task<bool> AnswerProject(RequestAnswerProjectViewModel model)
    {
        var project = _projectRepository.FirstOrDefaultItemAsync(x => x.Id == model.ProjectId).Result ??
                      throw new NotFoundException();

        if (!_projectRepository.Any(x => x.Class.Students
                .Select(x => x.Id)
                .Contains(CurrentUserId))) throw new FormValidationException(MessageId.UserNotInClass);

        if (DateTime.Now > project.EndDate) throw new FormValidationException(MessageId.DeadlineReached);

        var answerProject =
            _projectAnswerRepository.DeferredWhere(x => x.ProjectId == model.ProjectId && x.UserId == CurrentUserId)
                .FirstOrDefault() ?? new ProjectAnswer();

        if (!string.IsNullOrEmpty(answerProject.FileName)) _ = DeleteFile(answerProject.FileName);

        var filePathUpdate = UploadFile(model.File).Result;
        answerProject.FileName = string.IsNullOrEmpty(filePathUpdate) ? null : Path.GetFileName(filePathUpdate);
        answerProject.FileExtension = string.IsNullOrEmpty(filePathUpdate) ? null : Path.GetExtension(filePathUpdate);
        answerProject.UserId = CurrentUserId;
        answerProject.ProjectId = model.ProjectId;

        try
        {
            _projectAnswerRepository.UpdateAsync(answerProject, true);
            _logger.LogUpdateSuccess("ProjectAnswer", answerProject.Id);
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.LogUpdateError(e, "ProjectAnswer", answerProject.Id);
            throw new ErrorException();
        }
    }

    public Task<ResponseGetFileViewModel> GetProjectStudentAnswer(int projectId)
    {
        if (!_projectRepository.Any(x => x.Id == projectId)) throw new NotFoundException();

        var projectAnswer =
            _projectAnswerRepository.DeferredWhere(x => x.ProjectId == projectId && x.UserId == CurrentUserId)
                .FirstOrDefault();

        if (projectAnswer == null) throw new FormValidationException(MessageId.NotAnswered);

        return Task.FromResult(new ResponseGetFileViewModel
        {
            FileName = projectAnswer.FileName,
            MemoryStream = _uploader.Get(_configuration.GetSection("File:FileProjectAnswer").Value,
                projectAnswer.FileName, null).Result
        });
    }

    public Task<ResponseGetFileViewModel> GetProjectFile(string fileName)
    {
        var project =
            _projectRepository.DeferredWhere(x => x.FileName == fileName &&
                                                  x.Class.Students.Select(x => x.Id)
                                                      .Contains(CurrentUserId))
                .FirstOrDefault() ?? throw new NotFoundException();

        return Task.FromResult(new ResponseGetFileViewModel
        {
            FileName = project.FileName,
            MemoryStream = _uploader.Get(_configuration.GetSection("File:FileProject").Value,
                project.FileName, null).Result
        });
    }


    private async Task<string> UploadFile(IFormFile file)
    {
        var fileName = await _uploader.Upload(_configuration.GetSection("File:FileProjectAnswer").Value, file, null);
        return fileName;
    }

    private async Task<bool> DeleteFile(string fileName)
    {
        await _uploader.Delete(_configuration.GetSection("File:FileProjectAnswer").Value, fileName, null);
        return true;
    }
}