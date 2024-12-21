using Application.ViewModels.Public;
using Application.ViewModels.Ticket;

namespace Application.Services.Interface.StudentTicketService;

public interface ITicketStudentService
{
    Task<SelectOptionViewModel> GetClassTeacherId(int classId);

    Task<List<ResponseGetAllTicketViewModel>> GetAllTicket();

    Task<int> CreateTicket(RequestCreateTicketViewModel model);

    Task<int> SendMessage(RequestSendMessageViewModel model);

    Task<bool> CloseTicket(int ticketId);


    Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId);
    
    Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketTeacherMessages(int ticketId);


    Task<ResponseGetFileViewModel> GetTicketFile(string fileName);
}