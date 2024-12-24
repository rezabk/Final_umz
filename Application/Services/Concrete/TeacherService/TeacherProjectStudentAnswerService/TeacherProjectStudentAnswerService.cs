using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.TeacherService.TeacherProjectStudentAnswerService;
using Application.ViewModels.Practice;
using Application.ViewModels.Project;
using Application.ViewModels.Public;
using Common.Enums;
using Common.ExceptionType.CustomException;
using Domain.Entities.ProjectEntities;
using Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.TeacherService.TeacherProjectStudentAnswerService;

public class TeacherProjectStudentAnswerService : ServiceBase<TeacherProjectStudentAnswerService>,
    ITeacherProjectStudentAnswerService
{
    private readonly IRepository<ProjectAnswer> _projectAnswerRepository;
    private readonly IRepository<Project> _projectRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomLoggerService<TeacherProjectStudentAnswerService> _logger;
    private readonly ILiaraUploader _uploader;
    private readonly IConfiguration _configuration;

    public TeacherProjectStudentAnswerService(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor,
        UserManager<ApplicationUser> userManager, ILiaraUploader uploader, IConfiguration configuration,
        ICustomLoggerService<TeacherProjectStudentAnswerService> logger) : base(contextAccessor)

    {
        _projectAnswerRepository = unitOfWork.GetRepository<ProjectAnswer>();
        _projectRepository = unitOfWork.GetRepository<Project>();
        _userManager = userManager;
        _logger = logger;
        _uploader = uploader;
        _configuration = configuration;
    }

    public Task<List<UserAnsweredList>> GetAllUserAnswered(int projectId)
    {
        if (!_projectRepository.Any(x => x.Id == projectId && x.Class.Teacher.UserId == CurrentUserId))
            throw new FormValidationException(MessageId.AccessToClassDenied);

        var answeredUsers = _projectAnswerRepository.DeferredWhere(x =>
            x.ProjectId == projectId).Include(x => x.User);


        return Task.FromResult(answeredUsers.Select(x => new UserAnsweredList
        {
            UserId = x.UserId,
            StudentId = x.User.StudentId,
            FullName = x.User.FirstName + " " + x.User.LastName
        }).ToList());
    }

    public Task<ResponseGetStudentProjectAnswerViewModel> GetStudentProjectAnswerByUserId(int projectId, int userId)
    {
        var projectAnswer =
            _projectAnswerRepository.DeferredWhere(x => x.ProjectId == projectId && x.UserId == userId)
                .Include(x => x.User)
                .FirstOrDefault() ??
            throw new NotFoundException();

        return Task.FromResult(new ResponseGetStudentProjectAnswerViewModel
        {
            UserId = projectAnswer.UserId,
            StudentId = projectAnswer.User.StudentId,
            FullName = projectAnswer.User.FirstName + " " + projectAnswer.User.LastName,
            FileName = projectAnswer.FileName,
            Score = projectAnswer.Score
        });
    }

    public Task<ResponseGetFileViewModel> GetProjectAnswerFile(string fileName)
    {
        var file = _projectAnswerRepository
                       .DeferredWhere(x => x.FileName == fileName && x.Project.Class.Teacher.UserId == CurrentUserId)
                       .Select(x => x.FileName)
                       .FirstOrDefault() ??
                   throw new NotFoundException();

        return Task.FromResult(new ResponseGetFileViewModel
        {
            FileName = file,
            MemoryStream = _uploader.Get(_configuration.GetSection("File:FileProjectAnswer").Value,
                file, null).Result
        });
    }

    public Task<bool> ScoreProject(int projectId, int userId, double score)
    {
        var projectAnswer =
            _projectAnswerRepository.DeferredWhere(x =>
                    x.ProjectId == projectId && x.UserId == userId && x.Project.Class.Teacher.UserId == CurrentUserId)
                .FirstOrDefault() ?? throw new NotFoundException();

        projectAnswer.Score = score;

        try
        {
            _projectAnswerRepository.UpdateAsync(projectAnswer, true);
            _logger.LogUpdateSuccess("ProjectAnswer", projectAnswer.Id);
            return Task.FromResult(true);
        }
        catch (Exception e)
        {
            _logger.LogUpdateError(e, "ProjectAnswer", projectAnswer.Id);
            throw e ?? throw new ErrorException();
        }
    }
}