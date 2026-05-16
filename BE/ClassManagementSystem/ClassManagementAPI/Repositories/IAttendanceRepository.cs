using ClassManagementAPI.Models;

namespace ClassManagementAPI.Repositories
{
    public interface IAttendanceRepository
    {
        IEnumerable<Attendance> GetByClassAndDate(int classId, DateTime date, int? scheduleId = null);
        IEnumerable<Attendance> GetByClass(int classId);
        void UpsertAttendance(Attendance attendance);
        void SaveChanges();
    }
}
