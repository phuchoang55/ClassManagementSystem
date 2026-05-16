using ClassManagementAPI.Data;
using ClassManagementAPI.Models;

namespace ClassManagementAPI.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly AppDbContext _context;

        public ScheduleRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ClassSchedule> GetSchedulesByClassId(int classId)
        {
            return _context.Schedules.Where(s => s.ClassId == classId).ToList();
        }

        public ClassSchedule? GetScheduleById(int id)
        {
            return _context.Schedules.Find(id);
        }

        public void AddSchedule(ClassSchedule schedule)
        {
            _context.Schedules.Add(schedule);
        }

        public void UpdateSchedule(ClassSchedule schedule)
        {
            _context.Schedules.Update(schedule);
        }

        public void DeleteSchedule(ClassSchedule schedule)
        {
            _context.Schedules.Remove(schedule);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
