namespace ClassManagementAPI.DTOs
{
    public class AttendanceRecordDto
    {
        public int StudentId { get; set; }
        // "Present", "Absent", "Not yet"
        public string Status { get; set; } = "Not yet";
    }

    public class SubmitAttendanceDto
    {
        public DateTime Date { get; set; }
        public int? ScheduleId { get; set; }
        public List<AttendanceRecordDto> Records { get; set; } = new();
    }
}
