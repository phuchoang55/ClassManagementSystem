using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassManagementAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }
        public Class Class { get; set; }

        [Required]
        public int StudentId { get; set; }
        public User Student { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // "Present", "Absent", "Not yet"
        [Required]
        public string Status { get; set; } = "Not yet";

        public int? ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public ClassSchedule? Schedule { get; set; }
    }
}
