using ClassManagementAPI.DTOs;
using ClassManagementAPI.Models;
using ClassManagementAPI.Repositories;

namespace ClassManagementAPI.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IClassRepository _classRepo;

        public AttendanceService(IAttendanceRepository attendanceRepo, IClassRepository classRepo)
        {
            _attendanceRepo = attendanceRepo;
            _classRepo = classRepo;
        }

        private void ValidateTeacherOwnsClass(int teacherId, int classId)
        {
            var cls = _classRepo.GetClassById(classId);
            if (cls == null || cls.TeacherId != teacherId)
                throw new Exception("Lớp học không tồn tại hoặc bạn không có quyền.");
        }

        public IEnumerable<object> GetAttendanceByDate(int teacherId, int classId, DateTime date, int? scheduleId = null)
        {
            ValidateTeacherOwnsClass(teacherId, classId);
            var records = _attendanceRepo.GetByClassAndDate(classId, date, scheduleId);
            return records.Select(a => new
            {
                a.Id,
                a.StudentId,
                StudentName = a.Student.FullName,
                StudentEmail = a.Student.Email,
                a.Date,
                a.Status,
                a.ScheduleId
            });
        }

        public IEnumerable<object> GetAttendanceHistory(int teacherId, int classId)
        {
            ValidateTeacherOwnsClass(teacherId, classId);
            var records = _attendanceRepo.GetByClass(classId);
            // Group by date descending
            return records
                .GroupBy(a => a.Date.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Records = g.Select(a => new
                    {
                        a.Id,
                        a.StudentId,
                        StudentName = a.Student.FullName,
                        StudentEmail = a.Student.Email,
                        a.Status
                    })
                });
        }

        public void SubmitAttendance(int teacherId, int classId, SubmitAttendanceDto dto)
        {
            ValidateTeacherOwnsClass(teacherId, classId);

            foreach (var record in dto.Records)
            {
                var attendance = new Attendance
                {
                    ClassId = classId,
                    StudentId = record.StudentId,
                    Date = dto.Date.Date,
                    Status = record.Status,
                    ScheduleId = dto.ScheduleId
                };
                _attendanceRepo.UpsertAttendance(attendance);
            }
            _attendanceRepo.SaveChanges();
        }
    }
}
