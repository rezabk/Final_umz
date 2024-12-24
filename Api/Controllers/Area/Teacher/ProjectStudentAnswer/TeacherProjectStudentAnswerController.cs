using Application.Services.Interface.TeacherService.TeacherProjectStudentAnswerService;
using Application.ViewModels.Practice;
using Application.ViewModels.Project;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.ProjectStudentAnswer;

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/projectstudentanswer")]
public class TeacherProjectStudentAnswerController : BaseController
{
    private readonly ITeacherProjectStudentAnswerService _teacherProjectStudentAnswerService;

    public TeacherProjectStudentAnswerController(
        ITeacherProjectStudentAnswerService teacherProjectStudentAnswerService)
    {
        _teacherProjectStudentAnswerService = teacherProjectStudentAnswerService;
    }

    [HttpGet("[action]")]
    public async Task<List<UserAnsweredList>> GetAllUserAnswered(int projectId)
    {
        return await _teacherProjectStudentAnswerService.GetAllUserAnswered(projectId);
    }

    [HttpGet("[action]")]
    public async Task<ResponseGetStudentProjectAnswerViewModel> GetStudentProjectAnswerByUserId(int projectId,
        int userId)
    {
        return await _teacherProjectStudentAnswerService.GetStudentProjectAnswerByUserId(projectId, userId);
    }
    
      
    [HttpGet("[action]")]
    public Task<IResult> ServiceProjectAnswerFile(string fileName)
    {
        var response = _teacherProjectStudentAnswerService.GetProjectAnswerFile(fileName).Result;
        return Task.FromResult(Results.File(response.MemoryStream.ToArray(), "application/octet-stream",
            Path.GetFileName(response.FileName)));
    }
    
    [HttpPost("[action]")]
    public async Task<bool> ScoreProject( int projectId,int userId,double score)
    {
        return await _teacherProjectStudentAnswerService.ScoreProject(projectId, userId,score);
    }
}