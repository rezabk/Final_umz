using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.Ticket;

public class RequestSendMessageViewModel
{
    public int TicketId { get; set; }
    
    public string? Message { get; set; }
    
    public IFormFile? File { get; set; }
}