using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class UserBL : IUserBL
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        private readonly IAuthBL _authBL;

        public UserBL()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
            _authBL = BusinessLogicFactory.Instance.GetAuthBL();
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

        public bool ResetPassword(Guid id, string newPassword)
        {
            var user = _userRepository.GetById(id);
            if (user == null)
                return false;

            // Используем метод из AuthBL для создания хеша пароля
            string passwordHash = AuthService.CreatePasswordHash(newPassword);
            user.PasswordHash = passwordHash;
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