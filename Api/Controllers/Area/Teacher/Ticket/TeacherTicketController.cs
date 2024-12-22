using Application.Services.Interface.TeacherService.TeacherTicketService;
using Application.ViewModels.Public;
using Application.ViewModels.Ticket;
using Common.Enums.RolesManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Area.Teacher.Ticket;

[Area("Teacher")]
[Authorize(Roles = nameof(UserRolesEnum.Teacher))]
[Route("/api/teacher/ticket")]
public class TeacherTicketController : BaseController
{
    private readonly ITeacherTicketService _teacherTicketService;

    public TeacherTicketController(
        ITeacherTicketService teacherTicketService)
    {
        _teacherTicketService = teacherTicketService;
    }
    
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetAllTicket()
    {
        return await _teacherTicketService.GetAllTicket();
    }
    
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetAllTicketByClassId(int classId)
    {
        return await _teacherTicketService.GetAllTicketByClassId(classId);
    }
    
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetAllNewTicket( )
    {
        return await _teacherTicketService.GetAllNewTicket();
    }
    
    [HttpGet("[action]")]
    public async Task<List<ResponseGetAllTicketViewModel>> GetUserResponseTicket( )
    {
        return await _teacherTicketService.GetUserResponseTicket();
    }
    
    [HttpGet("[action]")]
    public async Task<List<SelectOptionViewModel>> GetAllClassStudents(int classId )
    {
        return await _teacherTicketService.GetAllClassStudents(classId);
    }
    
    [HttpPost("[action]")]
    public async Task<int> CreateTicket(RequestTeacherCreateTicketViewModel model )
    {
        return await _teacherTicketService.CreateTicket(model);
    }
    
    [HttpPost("[action]")]
    public async Task<int> SendMessage(RequestSendMessageViewModel model )
    {
        return await _teacherTicketService.SendMessage(model);
    }
    
    [HttpPost("[action]")]
    public async Task<bool> CloseTicket(int ticketId)
    {
        return await _teacherTicketService.CloseTicket(ticketId);
    }
    
    [HttpGet("[action]")]
    public async Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId)
    {
        return await _teacherTicketService.GetAllTicketMessages(ticketId);
    }
    
    [HttpGet("[action]")]
    public Task<IResult> ServeTicketFile(string fileName)
    {
        var response = _teacherTicketService.GetTicketFile(fileName).Result;

        return Task.FromResult(Results.File(response.MemoryStream.ToArray(), "application/octet-stream",
            Path.GetFileName(response.FileName)));
    }
}