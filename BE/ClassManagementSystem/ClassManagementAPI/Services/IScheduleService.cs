using ClassManagementAPI.DTOs;

namespace ClassManagementAPI.Services
{
    public interface IScheduleService
    {
        IEnumerable<ScheduleDto> GetSchedules(int teacherId, int classId);
        ScheduleDto CreateSchedule(int teacherId, int classId, CreateScheduleDto dto);
        ScheduleDto UpdateSchedule(int teacherId, int classId, int scheduleId, CreateScheduleDto dto);
        void DeleteSchedule(int teacherId, int classId, int scheduleId);
    }
}
