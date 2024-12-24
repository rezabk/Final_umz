using Application.ViewModels.Community;

namespace Application.Services.Interface.StudentCommunityService;

public interface IStudentCommunityService
{
    Task<ResponseGetCommunityViewModel> GetCommunityByClassId(int classId);

    Task<ResponseGetCommunityMessageViewModel> GetCommunityMessages(RequestGetCommunityMessageViewModel model);

    Task<int> SendMessage(RequestSendCommunityMessageViewModel model);

    Task<bool> DeleteMessage(int messageId);
}