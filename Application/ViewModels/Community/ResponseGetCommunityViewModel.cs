using Application.ViewModels.Public;
using Common.Enums.RolesManagment;

namespace Application.ViewModels.Community;

public class ResponseGetCommunityViewModel
{
    public int CommunityId { get; set; }

    public int TeacherId { get; set; }

    public string TeacherFullName { get; set; }

    public int ClassId { get; set; }

    public string ClassTitle { get; set; }
}

public record RequestGetCommunityMessageViewModel : RequestGetListViewModel
{
    public int CommunityId { get; set; }
}

public record ResponseGetCommunityMessageViewModel : ResponseGetListViewModel
{
    public int CommunityId { get; set; }

    public List<CommunityMessageViewModel> Messages { get; set; }
}

public class CommunityMessageViewModel
{
    public int Id { get; set; }

    public string Message { get; set; }

    public int SentByUserId { get; set; }

    public string SentByFullName { get; set; }

    public UserRolesEnum SentByRole { get; set; }

    public string SentByRoleTitle { get; set; }

    public string SentTime { get; set; }
}