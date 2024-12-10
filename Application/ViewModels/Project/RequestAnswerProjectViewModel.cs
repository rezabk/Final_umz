using Microsoft.AspNetCore.Http;

namespace Application.ViewModels.Project;

public class RequestAnswerProjectViewModel
{
    public int ProjectId { get; set; }

    public IFormFile File { get; set; }
}