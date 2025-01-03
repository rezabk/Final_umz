﻿using Application.DTO;
using Application.ViewModels.Account;
using Application.ViewModels.Profile;
using Domain.Entities.UserAgg;
using Domain.Entities.UserEntities;

namespace Application.Services.Interface.ProfileService.ProfileValidator;

public interface IProfileValidator
{
    public Task<bool> ValidateUpdateUser(ApplicationUser user,
        UpdateUserViewModel model);
}