using System.ComponentModel.DataAnnotations;

namespace ClassManagementAPI.DTOs
{
    public class AddStudentDto
    {
        [Required]
        [EmailAddress]
        public string StudentEmail { get; set; }
    }
}
