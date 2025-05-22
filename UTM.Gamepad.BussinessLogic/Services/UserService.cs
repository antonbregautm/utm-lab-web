using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        private readonly AuthService _authService = new AuthService();
        private readonly RoleService _roleService = new RoleService();

        public UserService()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
        }

        // Получение всех пользователей
        public List<User> GetAllUsers()
        {
            return _userRepository.GetAll().ToList();
        }

        // Получение пользователя по ID
        public User GetUserById(Guid id)
        {
            return _userRepository.GetById(id);
        }

        // Получение пользователя по Email
        public User GetUserByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }

        public User CreateUser(string fullName, string email, string password, string roleName = "User")
        {
            if (GetUserByEmail(email) != null)
            {
                return null;
            }

            var role = _roleRepository.GetByName(roleName);
            if (role == null)
            {
                role = new Role
                {
                    Name = roleName,
                    Description = $"Роль {roleName}"
                };
                _roleRepository.Add(role);
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = AuthService.CreatePasswordHash(password),
                RoleId = role.Id
            };

            return _userRepository.Add(user);
        }

        public bool UpdateUser(Guid id, string fullName, string email)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return false;

            var existingUser = GetUserByEmail(email);
            if (existingUser != null && existingUser.Id != id)
            {
                return false;
            }

            user.FullName = fullName;
            user.Email = email;

            _userRepository.Update(user);
            return true;
        }

        public bool ChangePassword(Guid id, string currentPassword, string newPassword)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return false;

            if (AuthService.CreatePasswordHash(currentPassword) != user.PasswordHash)
            {
                return false;
            }

            user.PasswordHash = AuthService.CreatePasswordHash(newPassword);
            _userRepository.Update(user);
            return true;
        }

        public bool ResetPassword(Guid id, string newPassword)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return false;

            user.PasswordHash = AuthService.CreatePasswordHash(newPassword);
            _userRepository.Update(user);
            return true;
        }

        public bool ChangeUserRole(Guid userId, Guid roleId)
        {
            var user = _userRepository.GetById(userId);
            if (user == null)
                return false;

            var role = _roleRepository.GetById(roleId);
            if (role == null)
                return false;

            user.RoleId = roleId;
            _userRepository.Update(user);
            return true;
        }

        public bool DeleteUser(Guid id)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return false;

            _userRepository.Delete(id);
            return true;
        }

        // Создание пользователя с указанной ролью по Id
        public User CreateUserWithRole(string fullName, string email, string password, Guid roleId)
        {
            // Проверяем, существует ли уже пользователь с таким email
            if (GetUserByEmail(email) != null)
            {
                return null;
            }

            // Проверяем существование роли
            var role = _roleRepository.GetById(roleId);
            if (role == null)
            {
                return null;
            }

            // Создаем нового пользователя
            var user = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = AuthService.CreatePasswordHash(password),
                RoleId = roleId
            };

            return _userRepository.Add(user);
        }
    }
} 