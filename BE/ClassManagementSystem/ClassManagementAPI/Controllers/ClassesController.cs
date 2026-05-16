using ClassManagementAPI.DTOs;
using ClassManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        private int GetTeacherId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int teacherId))
                return teacherId;
            throw new Exception("Lỗi xác thực.");
        }

        [HttpGet]
        public IActionResult GetClasses()
        {
            try
            {
                var teacherId = GetTeacherId();
                var classes = _classService.GetTeacherClasses(teacherId);
                return Ok(classes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateClass([FromBody] ClassCreateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var teacherId = GetTeacherId();
                var newClass = _classService.CreateClass(teacherId, request);
                return Ok(new { message = "Tạo lớp học thành công.", data = newClass });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateClass(int id, [FromBody] ClassUpdateDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var teacherId = GetTeacherId();
                var updatedClass = _classService.UpdateClass(teacherId, id, request);
                return Ok(new { message = "Cập nhật lớp học thành công.", data = updatedClass });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteClass(int id)
        {
            try
            {
                var teacherId = GetTeacherId();
                _classService.DeleteClass(teacherId, id);
                return Ok(new { message = "Xóa lớp học thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Student Management

        [HttpGet("{id}/students")]
        public IActionResult GetStudents(int id)
        {
            try
            {
                var teacherId = GetTeacherId();
                var students = _classService.GetStudents(teacherId, id);
                return Ok(students);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/students")]
        public IActionResult AddStudent(int id, [FromBody] AddStudentDto request)
        {
            try
            {
                var teacherId = GetTeacherId();
                var newStudent = _classService.AddStudent(teacherId, id, request.StudentEmail);
                return Ok(new { message = "Thêm học sinh thành công.", student = newStudent });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}/students/{studentId}")]
        public IActionResult RemoveStudent(int id, int studentId)
        {
            try
            {
                var teacherId = GetTeacherId();
                _classService.RemoveStudent(teacherId, id, studentId);
                return Ok(new { message = "Đã xóa học sinh khỏi lớp." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
