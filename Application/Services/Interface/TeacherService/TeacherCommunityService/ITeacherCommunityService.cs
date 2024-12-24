using Application.ViewModels.Community;

namespace Application.Services.Interface.TeacherService.TeacherCommunityService;

public interface ITeacherCommunityService
{
    Task<ResponseGetCommunityViewModel> GetCommunityByClassId(int classId);

    Task<ResponseGetCommunityMessageViewModel> GetCommunityMessages(RequestGetCommunityMessageViewModel model);
    
    Task<int> SendMessage(RequestSendCommunityMessageViewModel model);
    Task<bool> DeleteMessage(int messageId);
    
    Task<bool> DeleteAllCommunityMessage(int communityId);
}