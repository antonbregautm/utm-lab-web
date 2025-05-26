using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class UserApi
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        
        public UserApi()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
        }
        
        protected List<User> GetAllUsersFromDb()
        {
            return _userRepository.GetAll().ToList();
        }
        
        protected User GetUserByIdFromDb(Guid id)
        {
            return _userRepository.GetById(id);
        }
        
        protected User GetUserByEmailFromDb(string email)
        {
            return _userRepository.GetByEmail(email);
        }
        
        protected bool SaveUserToDb(User user)
        {
            try
            {
                _userRepository.Add(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool UpdateUserInDb(User user)
        {
            try
            {
                _userRepository.Update(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool DeleteUserFromDb(Guid id)
        {
            try
            {
                _userRepository.Delete(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        
        protected Role GetRoleById(int id)
        {
            return _roleRepository.GetById(Guid.Parse(id.ToString()));
        }
        
        protected Role GetRoleByGuidId(Guid id)
        {
            return _roleRepository.GetById(id);
        }
        
        protected bool UpdateUserWithoutPasswordInDb(User user)
        {
            if (user == null || user.Id == Guid.Empty)
            {
                return false;
            }
            
            var existingUser = GetUserByIdFromDb(user.Id);
            if (existingUser == null)
            {
                return false;
            }
            
            // Сохраняем пароль
            user.PasswordHash = existingUser.PasswordHash;
            
            return UpdateUserInDb(user);
        }
        
        protected bool UpdateUserBasicInfoInDb(Guid id, string fullName, string email)
        {
            var user = GetUserByIdFromDb(id);
            if (user == null)
            {
                return false;
            }
            
            user.FullName = fullName;
            user.Email = email;
            
            return UpdateUserWithoutPasswordInDb(user);
        }
        
        protected bool AssignRoleToUserInDb(Guid userId, int roleId)
        {
            var user = GetUserByIdFromDb(userId);
            if (user == null)
            {
                return false;
            }
            
            var role = GetRoleById(roleId);
            if (role == null)
            {
                return false;
            }
            
            user.RoleId = role.Id;
            user.Role = role;
            
            return UpdateUserInDb(user);
        }
        
        protected bool ResetUserPasswordInDb(Guid userId, string newPassword)
        {
            var user = GetUserByIdFromDb(userId);
            if (user == null || string.IsNullOrEmpty(newPassword))
            {
                return false;
            }
            
            user.PasswordHash = CreatePasswordHash(newPassword);
            
            return UpdateUserInDb(user);
        }
        
        protected bool ChangeUserRoleInDb(Guid userId, Guid roleId)
        {
            var user = GetUserByIdFromDb(userId);
            if (user == null)
            {
                return false;
            }
            
            user.RoleId = roleId;
            
            return UpdateUserInDb(user);
        }
        
        protected bool CreateUserInDb(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(password))
            {
                return false;
            }
            
            // Проверяем, не существует ли уже пользователь с таким email
            if (GetUserByEmailFromDb(user.Email) != null)
            {
                return false;
            }
            
            // Устанавливаем новый ID, если он не задан
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            
            // Хешируем пароль
            user.PasswordHash = CreatePasswordHash(password);
            
            return SaveUserToDb(user);
        }
        
        protected User CreateUserWithRoleInDb(string fullName, string email, string password, Guid roleId)
        {
            // Проверяем, не существует ли уже пользователь с таким email
            if (GetUserByEmailFromDb(email) != null)
            {
                return null;
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                PasswordHash = CreatePasswordHash(password),
                RoleId = roleId
            };
            
            if (SaveUserToDb(user))
            {
                return user;
            }
            
            return null;
        }
    }
} 