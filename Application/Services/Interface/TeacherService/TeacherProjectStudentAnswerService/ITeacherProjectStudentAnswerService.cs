using Application.ViewModels.Practice;
using Application.ViewModels.Project;
using Application.ViewModels.Public;

namespace Application.Services.Interface.TeacherService.TeacherProjectStudentAnswerService;

public interface ITeacherProjectStudentAnswerService
{
    Task<List<UserAnsweredList>> GetAllUserAnswered(int projectId);

    Task<ResponseGetStudentProjectAnswerViewModel> GetStudentProjectAnswerByUserId(int projectId, int userId);
    
    Task<ResponseGetFileViewModel> GetProjectAnswerFile(string fileName);

    Task<bool> ScoreProject(int projectId, int userId,double score);
}