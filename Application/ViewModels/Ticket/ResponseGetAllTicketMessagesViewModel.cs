using Common.Enums;
using Common.Enums.RolesManagment;

namespace Application.ViewModels.Ticket;

public class ResponseGetAllTicketMessagesViewModel
{
    public int TicketId { get; set; }

    public string Subject { get; set; }

    public string FileName { get; set; }

    public string CreateDate { get; set; }

    public int ClassId { get; set; }

    public string ClassTitle { get; set; }

    public string? CloseTime { get; set; }

    public TicketStatusEnum Status { get; set; }

    public string StatusTitle { get; set; }

    public List<MessagesViewModel> Messages { get; set; }
}

public class MessagesViewModel
{
    public string Message { get; set; }

    public string FileName { get; set; }

    public string SentByUserId { get; set; }

    public UserRolesEnum SentByRole { get; set; }

    public string SentByFullName { get; set; }

    public string SentTime { get; set; }
}