using Application.Services.Interface.ProfileService;
using Application.ViewModels.Profile;
using Application.ViewModels.Profile.ChangePassword;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    
    [HttpGet("[action]")]
    public async Task<ResponseGetProfileViewModel> Profile( )
    {
        return await _profileService.Profile();
    }

 
    [HttpPut("[action]")]
    public async Task<bool> ChangePassword([FromBody] ChangePasswordViewModel model)
    {
        return await _profileService.ChangePassword(model);
    }

    [HttpPut("[action]")]
    public async Task<bool> UpdateUser([FromForm] UpdateUserViewModel model)
    {
        return await _profileService.UpdateUser(model);
    }

    [HttpGet("[action]")]
    public Task<IActionResult> ServeUserImage(string fileName)
    {
        var path = _profileService.GetUserFileImage(fileName).Result;
        var file = System.IO.File.OpenRead(path);
        var newName = $"{Guid.NewGuid()}.{Path.GetExtension(path)}";
        return Task.FromResult<IActionResult>(File(file, "application/octet-stream", newName));
    }
}