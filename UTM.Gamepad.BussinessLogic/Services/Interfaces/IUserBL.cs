using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IUserBL
    {
        List<User> GetAllUsers();
        User GetUserById(Guid id);
        User GetUserByEmail(string email);
        bool UpdateUser(Guid id, string fullName, string email);
        bool ChangeUserRole(Guid userId, Guid roleId);
        bool ResetPassword(Guid id, string newPassword);
        bool DeleteUser(Guid id);
        User CreateUserWithRole(string fullName, string email, string password, Guid roleId);
    }
} 