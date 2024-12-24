using Application.ViewModels.Public;
using Application.ViewModels.Ticket;

namespace Application.Services.Interface.TeacherService.TeacherTicketService;

public interface ITeacherTicketService
{
    Task<List<ResponseGetAllTicketViewModel>> GetAllTicket();

    Task<List<ResponseGetAllTicketViewModel>> GetAllTicketByClassId(int classId);

    Task<List<ResponseGetAllTicketViewModel>> GetAllNewTicket();

    Task<List<ResponseGetAllTicketViewModel>> GetUserResponseTicket();

    Task<List<SelectOptionViewModel>> GetAllClassStudents(int classId);

    Task<int> CreateTicket(RequestTeacherCreateTicketViewModel model);
    
    Task<bool> CloseTicket(int ticketId);
    
    Task<int> SendMessage(RequestSendMessageViewModel model);

    Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId);
    
    Task<ResponseGetFileViewModel> GetTicketFile(string fileName);
}