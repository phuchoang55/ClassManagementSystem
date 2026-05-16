using ClassManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student,User")]
    public class StudentController : ControllerBase
    {
        private readonly IClassService _classService;

        public StudentController(IClassService classService)
        {
            _classService = classService;
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int userId))
                return userId;
            throw new Exception("Lỗi xác thực.");
        }

        [HttpGet("classes")]
        public IActionResult GetMyClasses()
        {
            try
            {
                var studentId = GetUserId();
                var classes = _classService.GetStudentEnrolledClasses(studentId);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("classes/{classId}")]
        public IActionResult GetClassDetail(int classId)
        {
            try
            {
                var studentId = GetUserId();
                var detail = _classService.GetStudentClassDetail(studentId, classId);
                return Ok(detail);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
