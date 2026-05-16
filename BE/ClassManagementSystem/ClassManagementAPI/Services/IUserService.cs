using ClassManagementAPI.DTOs;
using ClassManagementAPI.Models;

namespace ClassManagementAPI.Services
{
    public interface IUserService
    {
        IEnumerable<object> GetAllUsersFormatted();
        object CreateUser(AdminCreateUserDto request);
        object UpdateUser(int id, AdminEditUserDto request);
        void DeleteUser(int id);
    }
}
