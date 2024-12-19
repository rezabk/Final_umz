using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.StudentTicketService;
using Application.ViewModels.Public;
using Application.ViewModels.Ticket;
using Common.Enums;
using Common.Enums.RolesManagment;
using Common.ExceptionType.CustomException;
using Domain.Entities.ClassEntities;
using Domain.Entities.TicketEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.StudentTicketService;

public class TicketStudentService : ServiceBase<TicketStudentService>, ITicketStudentService
{
    private readonly IRepository<Class> _classRepository;
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<TicketMessage> _ticketMessageRepository;
    private readonly ICustomLoggerService<TicketStudentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILiaraUploader _uploader;
    private readonly IHttpContextAccessor _contextAccessor;

    public TicketStudentService(IUnitOfWork unitOfWork, ICustomLoggerService<TicketStudentService> logger,
        IConfiguration configuration, ILiaraUploader uploader,
        IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
        _classRepository = unitOfWork.GetRepository<Class>();
        _ticketRepository = unitOfWork.GetRepository<Ticket>();
        _ticketMessageRepository = unitOfWork.GetRepository<TicketMessage>();
        _logger = logger;
        _uploader = uploader;
        _configuration = configuration;
    }


    public Task<SelectOptionViewModel> GetClassTeacherId(int classId)
    {
        var classTeacher = _classRepository.DeferredWhere(x => x.Id == classId)
                               .Select(x => new { x.TeacherId, x.Teacher.User.FirstName, x.Teacher.User.LastName })
                               .FirstOrDefault() ??
                           throw new NotFoundException();

        return Task.FromResult(new SelectOptionViewModel
        {
            Id = classTeacher.TeacherId,
            Title = classTeacher.FirstName + " " + classTeacher.LastName
        });
    }

    public Task<int> CreateTicket(RequestCreateTicketViewModel model)
    {
        var classRoom = _classRepository.DeferredWhere(x => x.Id == model.ClassId).Select(x => new { x.TeacherId })
                            .FirstOrDefault() ??
                        throw new NotFoundException();

        // Check Student joined class
        if (!_classRepository.Any(x => x.Students.Select(x => x.Id).Contains(CurrentUserId)))
            throw new FormValidationException(MessageId.UserNotInClass);

        var newTicket = new Ticket
        {
            ClassId = model.ClassId,
            TeacherId = classRoom.TeacherId,
            Subject = model.Subject,
            Status = TicketStatusEnum.New,
            OpenByUserId = CurrentUserId,
            UserId = CurrentUserId,
            SentTime = DateTime.UtcNow.AddDays(3).Add(new TimeSpan(3, 30, 0)),
        };

        if (model.File != null)
        {
            var filePath = UploadFile(model.File).Result;
            newTicket.FileName = string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
            newTicket.FileExtension = string.IsNullOrEmpty(filePath) ? null : Path.GetExtension(filePath);
        }

        try
        {
            _ticketRepository.AddAsync(newTicket);
            _logger.LogAddSuccess("Ticket", newTicket.Id);
            return Task.FromResult(newTicket.Id);
        }
        catch (Exception exception)
        {
            _logger.LogAddError(exception, "Ticket");
            throw exception ?? throw new ErrorException();
        }
    }

    public Task<int> SendMessage(RequestSendMessageViewModel model)
    {
        var ticket = _ticketRepository.DeferredWhere(x => x.Id == model.TicketId).FirstOrDefault() ??
                     throw new NotFoundException();

        var newTicketMessage = new TicketMessage
        {
            SentTime = DateTime.UtcNow.AddDays(3).Add(new TimeSpan(3, 30, 0)),
            TicketId = model.TicketId,
            Message = model.Message,
            SentByUserId = CurrentUserId,
            SentByRole = UserRolesEnum.Student,
        };

        if (model.File != null)
        {
            var filePath = UploadFile(model.File).Result;
            newTicketMessage.FileName = string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
            newTicketMessage.FileExtension = string.IsNullOrEmpty(filePath) ? null : Path.GetExtension(filePath);
        }

        ticket.Status = TicketStatusEnum.UserResponse;

        try
        {
            _ticketMessageRepository.AddAsync(newTicketMessage, false);
            _ticketRepository.UpdateAsync(ticket, true);
            _logger.LogAddSuccess("TicketMessage", newTicketMessage.Id);
            _logger.LogUpdateSuccess("Ticket", ticket.Id);
            return Task.FromResult(newTicketMessage.Id);
        }
        catch (Exception exception)
        {
            _logger.LogAddError(exception, "TicketMessage");
            _logger.LogUpdateError(exception, "Ticket", ticket.Id);
            throw exception ?? throw new ErrorException();
        }
    }

    public Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId)
    {
        var ticket =
            _ticketRepository.DeferredWhere(x => x.Id == ticketId).Include(x => x.TicketMessages).FirstOrDefault() ??
            throw new NotFoundException();
        
        //todo
    }

    public Task<ResponseGetFileViewModel> GetTicketFile(string fileName)
    {
        var ticketFile = _ticketRepository.DeferredWhere(x => x.UserId == CurrentUserId).Select(x => x.FileName)
            .FirstOrDefault();

        var ticketMessageFile = _ticketMessageRepository.DeferredWhere(x => x.Ticket.UserId == CurrentUserId)
            .Select(x => x.FileName).FirstOrDefault();


        if (!string.IsNullOrWhiteSpace(ticketFile))
        {
            return Task.FromResult(new ResponseGetFileViewModel
            {
                FileName = ticketFile,
                MemoryStream = _uploader.Get(_configuration.GetSection("File:FileTicket").Value,
                    ticketFile, null).Result
            });
        }

        if (!string.IsNullOrWhiteSpace(ticketMessageFile))
        {
            return Task.FromResult(new ResponseGetFileViewModel
            {
                FileName = ticketMessageFile,
                MemoryStream = _uploader.Get(_configuration.GetSection("File:FileTicket").Value,
                    ticketMessageFile, null).Result
            });
        }

        throw new NotFoundException();
    }

    private async Task<string> UploadFile(IFormFile file)
    {
        var fileName = await _uploader.Upload(_configuration.GetSection("File:FileTicket").Value, file, null);
        return fileName;
    }
}