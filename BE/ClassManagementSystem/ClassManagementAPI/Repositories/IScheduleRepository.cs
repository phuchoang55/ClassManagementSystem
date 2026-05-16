using ClassManagementAPI.Models;

namespace ClassManagementAPI.Repositories
{
    public interface IScheduleRepository
    {
        IEnumerable<ClassSchedule> GetSchedulesByClassId(int classId);
        ClassSchedule? GetScheduleById(int id);
        void AddSchedule(ClassSchedule schedule);
        void UpdateSchedule(ClassSchedule schedule);
        void DeleteSchedule(ClassSchedule schedule);
        void SaveChanges();
    }
}
