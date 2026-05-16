using ClassManagementAPI.DTOs;
using ClassManagementAPI.Models;
using ClassManagementAPI.Repositories;

namespace ClassManagementAPI.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepo;
        private readonly IClassRepository _classRepo;

        public ScheduleService(IScheduleRepository scheduleRepo, IClassRepository classRepo)
        {
            _scheduleRepo = scheduleRepo;
            _classRepo = classRepo;
        }

        private void VerifyClassTeacher(int teacherId, int classId)
        {
            var cls = _classRepo.GetClassById(classId);
            if (cls == null) throw new Exception("Lớp học không tồn tại.");
            if (cls.TeacherId != teacherId) throw new Exception("Bạn không có quyền thao tác trên lớp học này.");
        }

        public IEnumerable<ScheduleDto> GetSchedules(int teacherId, int classId)
        {
            VerifyClassTeacher(teacherId, classId);
            var schedules = _scheduleRepo.GetSchedulesByClassId(classId);
            return schedules.Select(s => new ScheduleDto
            {
                Id = s.Id,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Room = s.Room
            });
        }

        public ScheduleDto CreateSchedule(int teacherId, int classId, CreateScheduleDto dto)
        {
            VerifyClassTeacher(teacherId, classId);
            if (dto.StartTime >= dto.EndTime) throw new Exception("Thời gian bắt đầu phải trước thời gian kết thúc.");

            var schedule = new ClassSchedule
            {
                ClassId = classId,
                DayOfWeek = dto.DayOfWeek,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Room = dto.Room
            };

            _scheduleRepo.AddSchedule(schedule);
            _scheduleRepo.SaveChanges();

            return new ScheduleDto
            {
                Id = schedule.Id,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room
            };
        }

        public ScheduleDto UpdateSchedule(int teacherId, int classId, int scheduleId, CreateScheduleDto dto)
        {
            VerifyClassTeacher(teacherId, classId);
            if (dto.StartTime >= dto.EndTime) throw new Exception("Thời gian bắt đầu phải trước thời gian kết thúc.");

            var schedule = _scheduleRepo.GetScheduleById(scheduleId);
            if (schedule == null || schedule.ClassId != classId) throw new Exception("Lịch học không tồn tại.");

            schedule.DayOfWeek = dto.DayOfWeek;
            schedule.StartTime = dto.StartTime;
            schedule.EndTime = dto.EndTime;
            schedule.Room = dto.Room;

            _scheduleRepo.UpdateSchedule(schedule);
            _scheduleRepo.SaveChanges();

            return new ScheduleDto
            {
                Id = schedule.Id,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Room = schedule.Room
            };
        }

        public void DeleteSchedule(int teacherId, int classId, int scheduleId)
        {
            VerifyClassTeacher(teacherId, classId);
            var schedule = _scheduleRepo.GetScheduleById(scheduleId);
            if (schedule == null || schedule.ClassId != classId) throw new Exception("Lịch học không tồn tại.");

            _scheduleRepo.DeleteSchedule(schedule);
            _scheduleRepo.SaveChanges();
        }
    }
}
