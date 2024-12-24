using Application.ViewModels.Practice;

namespace Application.ViewModels.Project;

public class ResponseGetStudentProjectAnswerViewModel : UserAnsweredList
{
    public string FileName { get; set; }

    public double Score { get; set; }
}