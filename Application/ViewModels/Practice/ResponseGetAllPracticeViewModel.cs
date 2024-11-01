namespace Application.ViewModels.Practice;

public class ShowPracticeByClassId
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public string ClassTitle { get; set; }

    public string? Description { get; set; }

    public string StartDate { get; set; }

    public string? EndDate { get; set; }

    public int PracticeQuestionsCount { get; set; }
}