using ClassManagementAPI.DTOs;
using ClassManagementAPI.Models;
using ClassManagementAPI.Repositories;

namespace ClassManagementAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public IEnumerable<object> GetAllUsersFormatted()
        {
            var users = _userRepository.GetAllUsers();
            return users.Select(u => new { u.Id, u.FullName, u.Email, u.Role });
        }

        public object CreateUser(AdminCreateUserDto request)
        {
            var existingUser = _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
                throw new Exception("Email đã được sử dụng.");

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _userRepository.AddUser(newUser);
            _userRepository.SaveChanges();

            return new { newUser.Id, newUser.FullName, newUser.Email, newUser.Role };
        }

        public object UpdateUser(int id, AdminEditUserDto request)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                throw new Exception("Không tìm thấy tài khoản.");

            if (user.Email != request.Email)
            {
                var existingUser = _userRepository.GetUserByEmail(request.Email);
                if (existingUser != null)
                    throw new Exception("Email đã được sử dụng bởi người khác.");
            }

            user.FullName = request.FullName;
            user.Email = request.Email;
            user.Role = request.Role;

            if (!string.IsNullOrEmpty(request.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            _userRepository.UpdateUser(user);
            _userRepository.SaveChanges();

            return new { user.Id, user.FullName, user.Email, user.Role };
        }

        public void DeleteUser(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user == null)
                throw new Exception("Không tìm thấy tài khoản.");

            _userRepository.DeleteUser(user);
            _userRepository.SaveChanges();
        }
    }
}
