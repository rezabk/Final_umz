using Application.ViewModels.Practice;
using Application.ViewModels.PracticeQuestion;

namespace Application.Services.Interface.StudentPracticeService;

public interface IStudentPracticeService
{
    Task<List<ShowPracticeByClassId>> GetAllPracticeByClassId(int classId);
    
    Task<List<ShowPracticeQuestionViewModel>> GetAllPracticeQuestionByPracticeId(int practiceId);

    Task<bool> AnswerPracticeQuestion(RequestAnswerPracticeQuestionViewModel model);
    
    Task<string> GetQuestionImage(string fileName);
}