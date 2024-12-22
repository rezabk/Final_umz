using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.Ticket;

public class RequestTeacherCreateTicketViewModel
{
    public int ClassId { get; set; }
    
    public int UserId { get; set; }

    public string Subject { get; set; }

    public IFormFile? File { get; set; }
}