using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.StudentTicketService;
using Application.ViewModels.Public;
using Application.ViewModels.Ticket;
using Common;
using Common.Enums;
using Common.Enums.RolesManagment;
using Common.ExceptionType.CustomException;
using Domain.Entities.ClassEntities;
using Domain.Entities.TicketEntities;
using Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.StudentTicketService;

public class TicketStudentService : ServiceBase<TicketStudentService>, ITicketStudentService
{
    private readonly IRepository<Class> _classRepository;
    private readonly IRepository<Ticket> _ticketRepository;
    private readonly IRepository<TicketMessage> _ticketMessageRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICustomLoggerService<TicketStudentService> _logger;
    private readonly IConfiguration _configuration;
    private readonly ILiaraUploader _uploader;
    private readonly IHttpContextAccessor _contextAccessor;

    public TicketStudentService(IUnitOfWork unitOfWork, ICustomLoggerService<TicketStudentService> logger,
        IConfiguration configuration, UserManager<ApplicationUser> userManager, ILiaraUploader uploader,
        IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
        _classRepository = unitOfWork.GetRepository<Class>();
        _ticketRepository = unitOfWork.GetRepository<Ticket>();
        _ticketMessageRepository = unitOfWork.GetRepository<TicketMessage>();
        _logger = logger;
        _uploader = uploader;
        _configuration = configuration;
        _userManager = userManager;
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

    public Task<List<ResponseGetAllTicketViewModel>> GetAllTicket()
    {
        var tickets = _ticketRepository
            .DeferredWhere(x => x.UserId == CurrentUserId)
            .Include(x => x.User)
            .Include(x => x.Class)
            .ToList();


        return Task.FromResult(tickets.Select(x => new ResponseGetAllTicketViewModel
        {
            TicketId = x.Id,
            Status = x.Status,
            StatusTitle = x.Status.GetEnumDescription(),
            Subject = x.Subject,
            ClassId = x.ClassId,
            ClassTitle = x.Class.Title,
            FileName = x.FileName,
            CreateDate = x.SentTime.ConvertMiladiToJalali(),
            CloseTime = x.CloseTime.ConvertMiladiToJalali(),
            ClosedByUserId = x.ClosedByUserId,
            ClosedByFullName = x.ClosedByUserId > 0
                ? GetUserFullName(x.ClosedByUserId)
                : null
        }).ToList());
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
            SentTime = DateTime.UtcNow,
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
        var ticket = _ticketRepository.DeferredWhere(x => x.Id == model.TicketId && x.Status != TicketStatusEnum.Closed)
                         .FirstOrDefault() ??
                     throw new NotFoundException();

        var newTicketMessage = new TicketMessage
        {
            SentTime = DateTime.UtcNow,
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

    public Task<bool> CloseTicket(int ticketId)
    {
        var ticket =
            _ticketRepository.DeferredWhere(x => x.UserId == CurrentUserId && x.Status != TicketStatusEnum.Closed)
                .FirstOrDefault() ?? throw new NotFoundException();

        ticket.Status = TicketStatusEnum.Closed;
        ticket.ClosedByUserId = CurrentUserId;

        try
        {
            _ticketRepository.UpdateAsync(ticket, true);
            _logger.LogUpdateSuccess("Ticket", ticket.Id);
            return Task.FromResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogUpdateError(exception, "Ticket", ticket.Id);
            throw exception ?? throw new ErrorException();
        }
    }

    public Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketMessages(int ticketId)
    {
        var ticket = _ticketRepository
            .DeferredWhere(x => x.Id == ticketId && x.UserId == CurrentUserId)
            .Include(x => x.TicketMessages.OrderByDescending(y => y.SentTime))
            .ThenInclude(x => x.SentByUser)
            .Include(x => x.Class)
            .FirstOrDefault() ?? throw new NotFoundException();


        return Task.FromResult(new ResponseGetAllTicketMessagesViewModel
        {
            TicketId = ticket.Id,
            Status = ticket.Status,
            StatusTitle = ticket.Status.GetEnumDescription(),
            Subject = ticket.Subject,
            ClassId = ticket.ClassId,
            ClassTitle = ticket.Class.Title,
            FileName = ticket.FileName,
            CreateDate = ticket.SentTime.ConvertMiladiToJalali(),
            CloseTime = ticket.CloseTime.ConvertMiladiToJalali(),
            ClosedByUserId = ticket.ClosedByUserId,
            ClosedByFullName = ticket.ClosedByUserId > 0
                ? GetUserFullName(ticket.ClosedByUserId)
                : null,
            Messages = ticket.TicketMessages.Select(x => new MessagesViewModel
            {
                Message = x.Message,
                FileName = x.FileName,
                SentByRole = x.SentByRole,
                SentTime = x.SentTime.ConvertMiladiToJalali(),
                SentByUserId = x.SentByUserId,
                SentByFullName = x.SentByUser.FirstName + " " + x.SentByUser.LastName
            }).ToList()
        });
    }

    public Task<ResponseGetAllTicketMessagesViewModel> GetAllTicketTeacherMessages(int ticketId)
    {
        var ticket = _ticketRepository
            .DeferredWhere(x => x.Id == ticketId && x.UserId == CurrentUserId)
            .Include(x =>
                x.TicketMessages.Where(y => y.SentByRole == UserRolesEnum.Teacher).OrderByDescending(y => y.SentTime))
            .ThenInclude(x => x.SentByUser)
            .Include(x => x.Class)
            .FirstOrDefault() ?? throw new NotFoundException();

        return Task.FromResult(new ResponseGetAllTicketMessagesViewModel
        {
            TicketId = ticket.Id,
            Status = ticket.Status,
            StatusTitle = ticket.Status.GetEnumDescription(),
            Subject = ticket.Subject,
            ClassId = ticket.ClassId,
            ClassTitle = ticket.Class.Title,
            FileName = ticket.FileName,
            CreateDate = ticket.SentTime.ConvertMiladiToJalali(),
            CloseTime = ticket.CloseTime.ConvertMiladiToJalali(),
            ClosedByUserId = ticket.ClosedByUserId,
            ClosedByFullName = ticket.ClosedByUserId > 0
                ? GetUserFullName(ticket.ClosedByUserId)
                : null,
            Messages = ticket.TicketMessages.Select(x => new MessagesViewModel
            {
                Message = x.Message,
                FileName = x.FileName,
                SentByRole = x.SentByRole,
                SentTime = x.SentTime.ConvertMiladiToJalali(),
                SentByUserId = x.SentByUserId,
                SentByFullName = x.SentByUser.FirstName + " " + x.SentByUser.LastName
            }).ToList()
        });
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

    private string GetUserFullName(int userId)
    {
        var user = _userManager.Users.FirstOrDefault(y => y.Id == userId);
        return user != null ? $"{user.FirstName} {user.LastName}" : null;
    }
}