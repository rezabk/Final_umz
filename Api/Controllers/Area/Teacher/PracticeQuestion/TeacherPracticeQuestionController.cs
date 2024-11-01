using Application.Services.Interface.TeacherService.TeacherPracticeQuestionService;
using Application.ViewModels.PracticeQuestion;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.PracticeQuestion;

//todo get all

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/practicequestion")]
public class TeacherPracticeQuestionController : BaseController
{
    private readonly ITeacherPracticeQuestionService _teacherPracticeQuestionService;

    public TeacherPracticeQuestionController(ITeacherPracticeQuestionService teacherPracticeQuestionService)
    {
        _teacherPracticeQuestionService = teacherPracticeQuestionService;
    }

    [HttpGet("[action]")]
    public async Task<List<ShowPracticeQuestionViewModel>> GetAllPracticeQuestionByPracticeId(int practiceId)
    {
        return await _teacherPracticeQuestionService.GetAllPracticeQuestionByPracticeId(practiceId);
    }

    [HttpPost("[action]")]
    public async Task<int> SetQuestion([FromForm] RequestSetQuestionViewModel model)
    {
        return await _teacherPracticeQuestionService.SetQuestion(model);
    }

    [HttpDelete("[action]")]
    public async Task<bool> RemoveQuestion(int practiceQuestionId)
    {
        return await _teacherPracticeQuestionService.RemoveQuestion(practiceQuestionId);
    }

    [HttpGet("[action]")]
    public Task<IActionResult> ServeQuestionImage(string fileName)
    {
        var path = _teacherPracticeQuestionService.GetQuestionImage(fileName).Result;
        var file = System.IO.File.OpenRead(path);
        var newName = $"{Guid.NewGuid()}.{Path.GetExtension(path)}";
        return Task.FromResult<IActionResult>(File(file, "application/octet-stream", newName));
    }
}