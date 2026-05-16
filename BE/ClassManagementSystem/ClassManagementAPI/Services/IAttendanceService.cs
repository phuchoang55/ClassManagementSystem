using ClassManagementAPI.DTOs;

namespace ClassManagementAPI.Services
{
    public interface IAttendanceService
    {
        IEnumerable<object> GetAttendanceByDate(int teacherId, int classId, DateTime date, int? scheduleId = null);
        IEnumerable<object> GetAttendanceHistory(int teacherId, int classId);
        void SubmitAttendance(int teacherId, int classId, SubmitAttendanceDto dto);
    }
}
