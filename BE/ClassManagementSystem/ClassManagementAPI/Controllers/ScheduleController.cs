using ClassManagementAPI.DTOs;
using ClassManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClassManagementAPI.Controllers
{
    [Route("api/classes/{classId}/schedules")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        private int GetTeacherId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out int id)) return id;
            throw new Exception("Lỗi xác thực.");
        }

        [HttpGet]
        public IActionResult GetSchedules(int classId)
        {
            try
            {
                var schedules = _scheduleService.GetSchedules(GetTeacherId(), classId);
                return Ok(schedules);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost]
        public IActionResult CreateSchedule(int classId, [FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var schedule = _scheduleService.CreateSchedule(GetTeacherId(), classId, dto);
                return Ok(schedule);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{scheduleId}")]
        public IActionResult UpdateSchedule(int classId, int scheduleId, [FromBody] CreateScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var schedule = _scheduleService.UpdateSchedule(GetTeacherId(), classId, scheduleId, dto);
                return Ok(schedule);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{scheduleId}")]
        public IActionResult DeleteSchedule(int classId, int scheduleId)
        {
            try
            {
                _scheduleService.DeleteSchedule(GetTeacherId(), classId, scheduleId);
                return Ok(new { message = "Xóa lịch học thành công." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
