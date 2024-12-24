using System.Runtime.InteropServices.ComTypes;
using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.StudentCommunityService;
using Application.ViewModels.Community;
using Common;
using Common.Enums;
using Common.Enums.RolesManagment;
using Common.ExceptionType.CustomException;
using DocumentFormat.OpenXml.InkML;
using Domain.Entities.CommunityEntities;
using Domain.Entities.UserEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Concrete.StudentCommunityService;

public class StudentCommunityService : ServiceBase<StudentCommunityService>, IStudentCommunityService
{
    private readonly IRepository<Community> _communityRepository;
    private readonly IRepository<CommunityMessage> _communityMessageRepository;
    private readonly ICustomLoggerService<StudentCommunityService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public StudentCommunityService(ICustomLoggerService<StudentCommunityService> logger,
        UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork
    ) : base(contextAccessor)
    {
        _communityRepository = unitOfWork.GetRepository<Community>();
        _communityMessageRepository = unitOfWork.GetRepository<CommunityMessage>();
        _logger = logger;
        _contextAccessor = contextAccessor;
        _userManager = userManager;
    }


    public Task<ResponseGetCommunityViewModel> GetCommunityByClassId(int classId)
    {
        var community = _communityRepository.DeferredWhere(x => x.ClassId == classId)
                            .Include(x => x.Class.Teacher).ThenInclude(x => x.User)
                            .Include(x => x.Class)
                            .ThenInclude(x => x.Students).FirstOrDefault() ??
                        throw new NotFoundException();

        if (!community.Class.Students.Select(x => x.Id).Contains(CurrentUserId))
            throw new FormValidationException(MessageId.UserNotInClass);

        return Task.FromResult(new ResponseGetCommunityViewModel
        {
            CommunityId = community.Id,
            ClassId = community.ClassId,
            ClassTitle = community.Class.Title,
            TeacherId = community.Class.TeacherId,
            TeacherFullName = community.Class.Teacher.User.FirstName + " " + community.Class.Teacher.User.LastName,
        });
    }

    public Task<ResponseGetCommunityMessageViewModel> GetCommunityMessages(RequestGetCommunityMessageViewModel model)
    {
        var messages = _communityMessageRepository.DeferredWhere(x => x.CommunityId == model.CommunityId
                                                                      && x.Community.Class.Students.Select(x => x.Id)
                                                                          .Contains(CurrentUserId))
            .Include(x => x.SentByUser)
            .OrderByDescending(x => x.Id);

        var messagesPaginated = messages.Paginate(model);

        return Task.FromResult(new ResponseGetCommunityMessageViewModel
        {
            CommunityId = model.CommunityId,
            Count = messagesPaginated.Count(),
            TotalCount = messages.Count(),
            CurrentPage = model.Page,
            Messages = messages.Select(x => new CommunityMessageViewModel
            {
                Id = x.Id,
                Message = x.Message,
                SentTime = x.SentTime.ConvertMiladiToJalaliUTC(true),
                SentByUserId = x.SentByUserId,
                SentByRole = x.SentByRole,
                SentByRoleTitle = x.SentByRole.GetEnumDescription(),
                SentByFullName = x.SentByUser.FirstName + " " + x.SentByUser.LastName
            }).ToList()
        });
    }

    public Task<int> SendMessage(RequestSendCommunityMessageViewModel model)
    {
        var community =
            _communityRepository.DeferredWhere(x => x.Id == model.CommunityId).Include(x => x.Class)
                .ThenInclude(x => x.Students).FirstOrDefault() ?? throw new NotFoundException();

        if (!community.Class.Students.Select(x => x.Id).Contains(CurrentUserId))
            throw new FormValidationException(MessageId.UserNotInClass);

        var newMessage = new CommunityMessage
        {
            CommunityId = community.Id,
            Message = model.Message,
            SentTime = DateTime.UtcNow,
            SentByUserId = CurrentUserId,
            SentByRole = UserRolesEnum.Student
        };
        try
        {
            _communityMessageRepository.AddAsync(newMessage);
            _logger.LogAddSuccess("CommunityMessage", newMessage.Id);
            return Task.FromResult(newMessage.Id);
        }
        catch (Exception exception)
        {
            _logger.LogAddError(exception, "CommunityMessage");
            throw exception ?? throw new ErrorException();
        }
    }

    public Task<bool> DeleteMessage(int messageId)
    {
        var message =
            _communityMessageRepository.DeferredWhere(x => x.Id == messageId && x.SentByUserId == CurrentUserId)
                .FirstOrDefault() ?? throw new NotFoundException();

        try
        {
            _communityMessageRepository.RemoveAsync(message, true);
            _logger.LogRemoveSuccess("CommunityMessage", message.Id);
            return Task.FromResult(true);
        }
        catch (Exception exception)
        {
            _logger.LogRemoveError(exception, "CommunityMessage", message.Id);
            throw exception ?? throw new ErrorException();
        }
    }
}