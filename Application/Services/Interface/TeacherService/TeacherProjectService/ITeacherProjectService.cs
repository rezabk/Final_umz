using Application.ViewModels.Project;

namespace Application.Services.Interface.TeacherService.TeacherProjectService;

public interface ITeacherProjectService
{
    Task<int> SetProject( RequestSetProjectViewModel model);
}