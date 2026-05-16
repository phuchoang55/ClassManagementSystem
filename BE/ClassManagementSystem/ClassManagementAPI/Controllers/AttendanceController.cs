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
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        private int GetTeacherId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int id)) return id;
            throw new Exception("Lỗi xác thực.");
        }

        // GET /api/attendance/{classId}?date=2024-01-15&scheduleId=3
        [HttpGet("{classId}")]
        public IActionResult GetByDate(int classId, [FromQuery] DateTime? date, [FromQuery] int? scheduleId)
        {
            try
            {
                var targetDate = date ?? DateTime.Today;
                var records = _attendanceService.GetAttendanceByDate(GetTeacherId(), classId, targetDate, scheduleId);
                return Ok(records);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // GET /api/attendance/{classId}/history
        [HttpGet("{classId}/history")]
        public IActionResult GetHistory(int classId)
        {
            try
            {
                var history = _attendanceService.GetAttendanceHistory(GetTeacherId(), classId);
                return Ok(history);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // POST /api/attendance/{classId}
        [HttpPost("{classId}")]
        public IActionResult Submit(int classId, [FromBody] SubmitAttendanceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                _attendanceService.SubmitAttendance(GetTeacherId(), classId, dto);
                return Ok(new { message = "Điểm danh thành công." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
