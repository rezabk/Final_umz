using Application.Services.Interface.StudentProjectService;
using Application.ViewModels.Project;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Student.Project;

[Area("Student")]
[Authorize(Roles = nameof(UserRolesEnum.Student))]
[Route("/api/student/project")]
public class StudentProjectController : BaseController
{
    private readonly IStudentProjectService _studentProjectService;
    
    public StudentProjectController(IStudentProjectService studentProjectService)
    {
        _studentProjectService = studentProjectService;
    }
    
    [HttpGet("[action]")]
    public async Task<List<ShowProjectViewModel>> GetAllProjectByClassId(int classId)
    {
        return await _studentProjectService.GetAllProjectByClassId(classId);
    }
    
    [HttpPost("[action]")]
    public async Task<bool> AnswerProject(RequestAnswerProjectViewModel model)
    {
        return await _studentProjectService.AnswerProject(model);
    }
    
    [HttpGet("[action]")]
    public Task<IResult> GetProjectStudentAnswer(int projectId)
    {
        var response = _studentProjectService.GetProjectStudentAnswer(projectId).Result;
        
        return Task.FromResult(Results.File(response.MemoryStream.ToArray(), "application/octet-stream",
            Path.GetFileName(response.FileName)));
    }
    
    
    
    [HttpGet("[action]")]
    public Task<IResult> ServeProjectFile(string fileName)
    {
        var response = _studentProjectService.GetProjectFile(fileName).Result;
        
        return Task.FromResult(Results.File(response.MemoryStream.ToArray(), "application/octet-stream",
            Path.GetFileName(response.FileName)));
    }
}