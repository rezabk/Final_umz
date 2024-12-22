using Application.Services.Interface.TeacherService.TeacherClassService;
using Application.ViewModels.Class;
using Application.ViewModels.Practice;
using Application.ViewModels.Public;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.Class;

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/class")]
public class TeacherClassController : BaseController
{
    private readonly ITeacherClassService _teacherClassService;

    public TeacherClassController(ITeacherClassService teacherClassService)
    {
        _teacherClassService = teacherClassService;
    }

    [HttpGet("[action]")]
    public async Task<List<SelectOptionViewModel>> GetAllClass()
    {
        return await _teacherClassService.GetAllClass();
    }

    [HttpPost("[action]")]
    public async Task<ResponseGetAllClassByFilterViewModel> GetAllClassByFilter(
        [FromBody] RequestGetAllClassByFilterViewModel model)
    {
        return await _teacherClassService.GetAllClassByFilter(model);
    }


    [HttpGet("[action]")]
    public async Task<List<UserAnsweredList>> GetAllClassStudents(int classId)

    {
        return await _teacherClassService.GetAllClassStudents(classId);
    }


    [HttpPost("[action]")]
    public async Task<int> SetClass([FromBody] RequestSetClassViewModel model)
    {
        return await _teacherClassService.SetClass(model);
    }

    [HttpDelete("[action]")]
    public async Task<bool> RemoveStudent(int classId, int userId)
    {
        return await _teacherClassService.RemoveStudent(classId, userId);
    }

    [HttpDelete("[action]")]
    public async Task<bool> RemoveClass(int classId)
    {
        return await _teacherClassService.RemoveClass(classId);
    }
}