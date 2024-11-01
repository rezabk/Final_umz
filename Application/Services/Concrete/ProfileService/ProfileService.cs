using Application.Cross.Interface;
using Application.IRepositories;
using Application.Services.Base;
using Application.Services.Interface.Logger;
using Application.Services.Interface.ProfileService;
using Application.Services.Interface.ProfileService.ProfileValidator;
using Application.ViewModels.Profile;
using Application.ViewModels.Profile.ChangePassword;
using Common;
using Common.Enums;
using Common.ExceptionType.CustomException;
using Domain.Entities;
using Domain.Entities.UserAgg;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Services.Concrete.ProfileService;

public class ProfileService : ServiceBase<ProfileService>, IProfileService
{
    private readonly ICustomLoggerService<ProfileService> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUploader _uploader;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _hostingEnvironment;
    private readonly IProfileValidator _profileValidator;

    public ProfileService(IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork,
        ICustomLoggerService<ProfileService> logger, IProfileValidator profileValidator,
        IUploader uploader,
        IConfiguration configuration, IWebHostEnvironment hostingEnvironment,
        UserManager<ApplicationUser> userManager) : base(contextAccessor)
    {
        _userManager = userManager;
        _logger = logger;
        _uploader = uploader;
        _configuration = configuration;
        _hostingEnvironment = hostingEnvironment;
        _profileValidator = profileValidator;
    }

    public async Task<ResponseGetProfileViewModel> Profile()
    {
        var user = await _userManager.Users.Where(x => x.Id == CurrentUserId).FirstOrDefaultAsync() ??
                   throw new NotFoundException();

        return new ResponseGetProfileViewModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName,
            Birthday = user.Birthday.ConvertMiladiToJalali(),
            File = user.ProfileImageFileName
        };
    }

    public async Task<bool> UpdateUser(UpdateUserViewModel model)
    {
        var user = await _userManager.Users.Where(x => x.Id == CurrentUserId).FirstOrDefaultAsync() ??
                   throw new NotFoundException();

         await _profileValidator.ValidateUpdateUser(user, model);
        
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Birthday = model.Birthday.ConvertJalaliToMiladi();
        if (!string.IsNullOrEmpty(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;
        if (!string.IsNullOrEmpty(model.Email)) user.Email = model.Email;

        if (model.File != null)
        {
            var filePath = UploadFile(model.File);
            user.ProfileImageFileName = string.IsNullOrEmpty(filePath) ? null : Path.GetFileName(filePath);
            user.ProfileImageFileExtension = string.IsNullOrEmpty(filePath) ? null : Path.GetExtension(filePath);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new ErrorException();

        return true;
    }


    public async Task<bool> ChangePassword(ChangePasswordViewModel model)
    {
        var user = await _userManager.Users.Where(x => x.Id == CurrentUserId).FirstOrDefaultAsync() ??
                   throw new NotFoundException();
        if (user.PasswordHash == null) throw new FormValidationException(MessageId.NoPassword);

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
        if (!result.Succeeded) throw new Exception(result.Errors.ToString());

        return true;
    }

    public async Task<string> GetUserFileImage(string fileName)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.ProfileImageFileName == fileName)
                   ?? throw new NotFoundException();

        var folderPath = _hostingEnvironment.ContentRootPath +
                         _configuration.GetSection("File:ImageFile").Value;
        var fullPath = Path.Combine(folderPath, user.ProfileImageFileName);

        return fullPath;
    }

    private string UploadFile(IFormFile file)
    {
        var fullFilePath = _uploader.UploadFile(file,
            _hostingEnvironment.ContentRootPath + _configuration.GetSection("File:ImageFile").Value,
            "").Result;

        return fullFilePath;
    }
}