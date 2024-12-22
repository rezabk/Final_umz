using Application.ViewModels.Class;
using Application.ViewModels.Practice;
using Application.ViewModels.Public;

namespace Application.Services.Interface.TeacherService.TeacherClassService;

public interface ITeacherClassService
{
    Task<List<SelectOptionViewModel>> GetAllClass();
    Task<ResponseGetAllClassByFilterViewModel> GetAllClassByFilter(RequestGetAllClassByFilterViewModel model);

    Task<List<UserAnsweredList>> GetAllClassStudents(int classId);
    Task<int> SetClass(RequestSetClassViewModel model);

    Task<bool> RemoveStudent(int classId,int userId);
    Task<bool> RemoveClass(int classId);
}