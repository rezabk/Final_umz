using Application.ViewModels.Profile;
using Application.ViewModels.Profile.ChangePassword;

namespace Application.Services.Interface.ProfileService;

public interface IProfileService
{

    Task<ResponseGetProfileViewModel> Profile();
    Task<bool> UpdateUser(UpdateUserViewModel model);
    

    Task<bool> ChangePassword(ChangePasswordViewModel model);
    
    Task<string> GetUserFileImage(string fileName);
}