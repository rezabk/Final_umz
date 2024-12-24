using Application.ViewModels.Admin.AdminRoleService;
using Application.ViewModels.Practice;
using Application.ViewModels.Public;

namespace Application.Services.Interface.Admin.AdminRoleService;

public interface IAdminRoleService
{
    Task<bool> AssignTeacher( RequestAssignTeacherByAdminViewModel model);

    Task<ResponseGetUserInfoViewModel> GetAllUserInfoByFilter(RequestGetListViewModel model);
}