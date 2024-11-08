

using Application.Services.Interface.TeacherService.TeacherProjectService;
using Application.ViewModels.Project;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.Project;

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/project")]
public class TeacherProjectController  :BaseController
{
    private readonly ITeacherProjectService _teacherProjectService;

    public TeacherProjectController(ITeacherProjectService teacherProjectService)
    {
        _teacherProjectService = teacherProjectService;
    }
    
        
    [HttpPost("[action]")]
    public async Task<int> SetProject([FromForm] RequestSetProjectViewModel model)
    {
        return await _teacherProjectService.SetProject(model);
    }
}