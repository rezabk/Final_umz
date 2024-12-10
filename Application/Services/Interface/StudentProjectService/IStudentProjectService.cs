using Application.ViewModels.Project;
using Application.ViewModels.Public;

namespace Application.Services.Interface.StudentProjectService;

public interface IStudentProjectService
{
    Task<List<ShowProjectViewModel>> GetAllProjectByClassId(int classId);

    Task<bool> AnswerProject(RequestAnswerProjectViewModel model);


    Task<ResponseGetFileViewModel> GetProjectStudentAnswer(int projectId);
    Task<ResponseGetFileViewModel> GetProjectFile(string fileName);
    
    
}