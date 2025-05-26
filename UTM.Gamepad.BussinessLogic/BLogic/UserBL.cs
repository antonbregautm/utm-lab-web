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
            return UpdateUserWithoutPasswordInDb(user);
        }
        
        public bool UpdateUser(Guid id, string fullName, string email)
        {
            return UpdateUserBasicInfoInDb(id, fullName, email);
        }
        
        public bool DeleteUser(Guid id)
        {
            return DeleteUserFromDb(id);
        }
        
        public bool AssignRole(Guid userId, int roleId)
        {
            return AssignRoleToUserInDb(userId, roleId);
        }
        
        public bool ResetPassword(Guid userId, string newPassword)
        {
            return ResetUserPasswordInDb(userId, newPassword);
        }
        
        public bool ChangeUserRole(Guid userId, Guid roleId)
        {
            return ChangeUserRoleInDb(userId, roleId);
        }
        
        public bool CreateUser(User user, string password)
        {
            return CreateUserInDb(user, password);
        }
        
        public User CreateUserWithRole(string fullName, string email, string password, Guid roleId)
        {
            return CreateUserWithRoleInDb(fullName, email, password, roleId);
        }
    }
} 