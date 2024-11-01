﻿using Application.ViewModels.PracticeQuestion;

namespace Application.Services.Interface.TeacherService.TeacherPracticeQuestionService;

public interface ITeacherPracticeQuestionService
{
    Task<List<ShowPracticeQuestionViewModel>> GetAllPracticeQuestionByPracticeId(int practiceId);
    Task<int> SetQuestion(RequestSetQuestionViewModel model);

    Task<bool> RemoveQuestion(int practiceQuestionId);
    
    Task<string> GetQuestionImage(string fileName);
}