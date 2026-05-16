using System.ComponentModel.DataAnnotations;

namespace ClassManagementAPI.DTOs
{
    public class AdminEditUserDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
