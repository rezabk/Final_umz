using Application.Services.Interface.StudentCommunityService;
using Application.ViewModels.Community;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Student.Community;

[Area("Student")]
[Authorize(Roles = nameof(UserRolesEnum.Student))]
[Route("/api/student/community")]
public class StudentCommunityController : BaseController
{
    private readonly IStudentCommunityService _studentCommunityService;

    public StudentCommunityController(IStudentCommunityService studentCommunityService)
    {
        _studentCommunityService = studentCommunityService;
    }

    [HttpGet("[action]")]
    public async Task<ResponseGetCommunityViewModel> GetCommunityByClassId(int classId)
    {
        return await _studentCommunityService.GetCommunityByClassId(classId);
    }
    
    [HttpPost("[action]")]
    public async Task<ResponseGetCommunityMessageViewModel> GetCommunityMessages(RequestGetCommunityMessageViewModel model)
    {
        return await _studentCommunityService.GetCommunityMessages(model);
    }

    [HttpPost("[action]")]
    public async Task<int> SendMessage(RequestSendCommunityMessageViewModel model)
    {
        return await _studentCommunityService.SendMessage(model);
    }

    [HttpDelete("[action]")]
    public async Task<bool> DeleteMessage(int messageId)
    {
        return await _studentCommunityService.DeleteMessage(messageId);
    }
}