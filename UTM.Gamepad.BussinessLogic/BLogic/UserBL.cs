using System;
using System.Collections.Generic;
using UTM.Gamepad.BussinessLogic.Core;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.BLogic
{
    public class UserBL : UserApi, IUserBL
    {
        public UserBL() : base() { }
        
        public List<User> GetAllUsers()
        {
            return GetAllUsersFromDb();
        }
        
        public User GetUserById(Guid id)
        {
            return GetUserByIdFromDb(id);
        }
        
        public User GetUserByEmail(string email)
        {
            return GetUserByEmailFromDb(email);
        }
        
        public bool UpdateUser(User user)
        {
            if (user == null || user.Id == Guid.Empty)
            {
                return false;
            }
            
            var existingUser = GetUserById(user.Id);
            if (existingUser == null)
            {
                return false;
            }
            
            // Сохраняем пароль
            user.PasswordHash = existingUser.PasswordHash;
            
            return UpdateUserInDb(user);
        }
        
        public bool UpdateUser(Guid id, string fullName, string email)
        {
            var user = GetUserById(id);
            if (user == null)
            {
                return false;
            }
            
            user.FullName = fullName;
            user.Email = email;
            
            return UpdateUser(user);
        }
        
        public bool DeleteUser(Guid id)
        {
            return DeleteUserFromDb(id);
        }
        
        public bool AssignRole(Guid userId, int roleId)
        {
            var user = GetUserById(userId);
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
        
        public bool ResetPassword(Guid userId, string newPassword)
        {
            var user = GetUserById(userId);
            if (user == null || string.IsNullOrEmpty(newPassword))
            {
                return false;
            }
            
            user.PasswordHash = CreatePasswordHash(newPassword);
            
            return UpdateUserInDb(user);
        }
        
        public bool ChangeUserRole(Guid userId, Guid roleId)
        {
            var user = GetUserById(userId);
            if (user == null)
            {
                return false;
            }
            
            user.RoleId = roleId;
            
            return UpdateUserInDb(user);
        }
        
        public bool CreateUser(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(password))
            {
                return false;
            }
            
            // Проверяем, не существует ли уже пользователь с таким email
            if (GetUserByEmail(user.Email) != null)
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
        
        public User CreateUserWithRole(string fullName, string email, string password, Guid roleId)
        {
            // Проверяем, не существует ли уже пользователь с таким email
            if (GetUserByEmail(email) != null)
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