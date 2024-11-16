using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.TeacherService.TeacherPracticeStudentAnswerService;
using Application.ViewModels.Practice;
using Common.ExceptionType.CustomException;
using Domain.Entities.PracticeEntities;
using Domain.Entities.UserAgg;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Concrete.TeacherService.TeacherPracticeStudentAnswerService;

public class TeacherPracticeStudentAnswerService : ServiceBase<TeacherPracticeStudentAnswerService>,
    ITeacherPracticeStudentAnswerService
{
    private readonly IRepository<Practice> _practiceRepository;
    private readonly IRepository<PracticeQuestionAnswer> _practiceQuestionAnswerRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public TeacherPracticeStudentAnswerService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor,
        UserManager<ApplicationUser> userManager) : base(
        contextAccessor)
    {
        _practiceRepository = unitOfWork.GetRepository<Practice>();
        _practiceQuestionAnswerRepository = unitOfWork.GetRepository<PracticeQuestionAnswer>();
        _userManager = userManager;
    }


    public Task<ShowPracticeAnswer> GetAllPracticeAnswerByUserId(int practiceId, int userId)
    {
        var answers = _practiceQuestionAnswerRepository.DeferredWhere(x =>
                x.UserId == userId && x.PracticeQuestion.Practice.Id == practiceId &&
                x.PracticeQuestion.Practice.Class.Teacher.UserId == CurrentUserId).Include(x => x.PracticeQuestion)
            .ThenInclude(x => x.Practice)
            .Include(x => x.User);

        var practice =
            _practiceRepository.DeferredWhere(x => x.Class.Teacher.UserId == CurrentUserId && x.Id == practiceId)
                .FirstOrDefault() ??
            throw new NotFoundException();

        var user = _userManager.Users.FirstOrDefault(x => x.Id == userId) ?? throw new NotFoundException();

        return Task.FromResult(new ShowPracticeAnswer
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PracticeId = practice.Id,
            PracticeTitle = practice.Title,
            StudentNumber = user.StudentId,
            QuestionAnswerObjects = answers.Select(x => new PracticeQuestionAnswerObject
            {
                QuestionId = x.PracticeQuestionId,
                QuestionTitle = x.PracticeQuestion.Title,
                Answer = x.Description,
                Score = x.Score
            }).ToList()
        });
    }

    public Task<List<UserAnsweredList>> GetAllUserAnswered(int practiceId)
    {
        var practice =
            _practiceRepository.DeferredWhere(x => x.Class.Teacher.UserId == CurrentUserId && x.Id == practiceId)
                .Include(x => x.PracticeQuestions).ThenInclude(x => x.PracticeQuestionAnswers).ThenInclude(x => x.User)
                .FirstOrDefault() ??
            throw new NotFoundException();

        return Task.FromResult(practice.PracticeQuestions.SelectMany(x => x.PracticeQuestionAnswers).Select(x =>
            new UserAnsweredList
            {
                StudentId = x.User.StudentId,
                UserId = x.UserId,
                FullName = x.User.FirstName + " " + x.User.LastName
            }).ToList());
    }
}