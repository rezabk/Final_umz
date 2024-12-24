using Application.Services.Interface.TeacherService.TeacherCommunityService;
using Application.ViewModels.Community;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.Community;

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/community")]
public class TeacherCommunityController: BaseController
{
    private readonly ITeacherCommunityService _teacherCommunityService;

    public TeacherCommunityController(ITeacherCommunityService teacherCommunityService)
    {
        _teacherCommunityService = teacherCommunityService;
    }
    
    [HttpGet("[action]")]
    public async Task<ResponseGetCommunityViewModel> GetCommunityByClassId(int classId)
    {
        return await _teacherCommunityService.GetCommunityByClassId(classId);
    }
    
      
    [HttpPost("[action]")]
    public async Task<ResponseGetCommunityMessageViewModel> GetCommunityMessages(RequestGetCommunityMessageViewModel model)
    {
        return await _teacherCommunityService.GetCommunityMessages(model);
    }
    
    
    [HttpPost("[action]")]
    public async Task<int> SendMessage(RequestSendCommunityMessageViewModel model)
    {
        return await _teacherCommunityService.SendMessage(model);
    }

    [HttpDelete("[action]")]
    public async Task<bool> DeleteMessage(int messageId)
    {
        return await _teacherCommunityService.DeleteMessage(messageId);
    }
    
    [HttpDelete("[action]")]
    public async Task<bool> DeleteAllCommunityMessage(int communityId)
    {
        return await _teacherCommunityService.DeleteAllCommunityMessage(communityId);
    }
}