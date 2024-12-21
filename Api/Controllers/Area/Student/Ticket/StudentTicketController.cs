using Application.Services.Interface.StudentTicketService;
using Application.ViewModels.Public;
using Application.ViewModels.Ticket;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Student.Ticket;

[Area("Student")]
[Authorize(Roles = nameof(UserRolesEnum.Student))]
[Route("/api/student/ticket")]
public class StudentTicketController : BaseController
{
    private readonly ITicketStudentService _ticketStudentService;

    public StudentTicketController(ITicketStudentService ticketStudentService)
    {
        _ticketStudentService = ticketStudentService;
    }

    [HttpGet("[action]")]
    public async Task<SelectOptionViewModel> GetClassTeacherId(int classId)
    {
        return await _ticketStudentService.GetClassTeacherId(classId);
    }
    
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetAllTicket()
    {
        return await _ticketStudentService.GetAllTicket();
    }
    
      
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetAllNewTicket()
    {
        return await _ticketStudentService.GetAllNewTicket();
    }

    [HttpPost("[action]")]
    public async Task<int> CreateTicket([FromForm] RequestCreateTicketViewModel model)
    {
        return await _ticketStudentService.CreateTicket(model);
    }

    [HttpPost("[action]")]
    public async Task<int> SendMessage([FromForm] RequestSendMessageViewModel model)
    {
        return await _ticketStudentService.SendMessage(model);
    }
    
    [HttpPost("[action]")]
    public async Task<bool> CloseTicket(int ticketId)
    {
        return await _ticketStudentService.CloseTicket(ticketId);
    }
    
    [HttpGet("[action]")]
    public async Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId)
    {
        return await _ticketStudentService.GetAllTicketMessages(ticketId);
    }
    
    [HttpGet("[action]")]
    public async Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketTeacherMessages(int ticketId)
    {
        return await _ticketStudentService.GetAllTicketTeacherMessages(ticketId);
    }

    [HttpGet("[action]")]
    public Task<IResult> ServeTicketFile(string fileName)
    {
        var response = _ticketStudentService.GetTicketFile(fileName).Result;

        return Task.FromResult(Results.File(response.MemoryStream.ToArray(), "application/octet-stream",
            Path.GetFileName(response.FileName)));
    }
}