using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.Ticket;

public class RequestCreateTicketViewModel
{
    public int ClassId { get; set; }

    public string Subject { get; set; }

    public IFormFile? File { get; set; }
}