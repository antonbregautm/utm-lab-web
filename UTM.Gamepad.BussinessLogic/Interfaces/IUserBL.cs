using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Interfaces
{
    public interface IUserBL
    {
        List<User> GetAllUsers();
        User GetUserById(Guid id);
        User GetUserByEmail(string email);
        bool UpdateUser(User user);
        bool UpdateUser(Guid id, string fullName, string email);
        bool DeleteUser(Guid id);
        bool AssignRole(Guid userId, int roleId);
        bool ResetPassword(Guid userId, string newPassword);
        bool ChangeUserRole(Guid userId, Guid roleId);
        bool CreateUser(User user, string password);
        User CreateUserWithRole(string fullName, string email, string password, Guid roleId);
    }
} 