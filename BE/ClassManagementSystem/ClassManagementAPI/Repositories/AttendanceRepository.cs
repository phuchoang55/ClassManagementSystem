using ClassManagementAPI.Data;
using ClassManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassManagementAPI.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Attendance> GetByClassAndDate(int classId, DateTime date, int? scheduleId = null)
        {
            var dateOnly = date.Date;
            var query = _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.ClassId == classId && a.Date.Date == dateOnly);
                
            if (scheduleId.HasValue)
            {
                query = query.Where(a => a.ScheduleId == scheduleId.Value);
            }
                
            return query.ToList();
        }

        public IEnumerable<Attendance> GetByClass(int classId)
        {
            return _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.ClassId == classId)
                .OrderByDescending(a => a.Date)
                .ToList();
        }

        public void UpsertAttendance(Attendance attendance)
        {
            var existing = _context.Attendances.FirstOrDefault(a =>
                a.ClassId == attendance.ClassId &&
                a.StudentId == attendance.StudentId &&
                a.Date.Date == attendance.Date.Date &&
                a.ScheduleId == attendance.ScheduleId);

            if (existing != null)
                existing.Status = attendance.Status;
            else
                _context.Attendances.Add(attendance);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
