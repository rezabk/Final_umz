//todo 

// using Common.Enums.RolesManagment;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Api.Controllers.Area.Teacher.PracticeQuestionStudentAnswer;
//
// [Area("Teacher")]
// [Authorize(Roles = nameof(UserRolesEnum.Teacher))]
// [Route("/api/teacher/practicequestionstudentanswer")]
// public class PracticeStudentAnswerController : BaseController
// {
//     private readonly ITeacherPracticeStudentAnswerService _teacherPracticeStudentAnswerService;
//
//     public PracticeStudentAnswerController(
//         ITeacherPracticeStudentAnswerService teacherPracticeStudentAnswerService)
//     {
//         _teacherPracticeStudentAnswerService = teacherPracticeStudentAnswerService;
//     }
//     
//     
//     [HttpGet("[action]")]
//     public async Task<List<ShowPracticeAnswer>> GetAllPracticeAnswerByUserId(int practiceId,int userId)
//     {
//         return await _teacherPracticeStudentAnswerService.GetAllPracticeAnswerByUserId(practiceId,userId);
//     }
//     
//     
// }