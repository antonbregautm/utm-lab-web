using System;
using System.Linq;
using System.Web.Security;
using UTM.Gamepad.Application.Services;

namespace UTM.Gamepad.Web.App_Start
{
    public class CustomRoleProvider : RoleProvider
    {
        private readonly UserService _userService = new UserService();

        public override string ApplicationName 
        { 
            get => "UTM.Gamepad.Web"; 
            set { } 
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            var user = _userService.GetUserByEmail(username);
            if (user == null || user.Role == null)
                return false;
                
            return user.Role.Name == roleName;
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = _userService.GetUserByEmail(username);
            if (user == null || user.Role == null)
                return new string[] { };
                
            return new[] { user.Role.Name };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var users = _userService.GetAllUsers();
            if (users == null)
                return new string[] { };
                
            return users
                .Where(u => u.Role != null && u.Role.Name == roleName)
                .Select(u => u.Email)
                .ToArray();
        }

        public override bool RoleExists(string roleName)
        {
            var roleService = new RoleService();
            var roles = roleService.GetAllRoles();
            return roles.Any(r => r.Name == roleName);
        }

        // Ниже приведены методы, которые мы не используем, но должны реализовать
        
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            var roleService = new RoleService();
            return roleService.GetAllRoles().Select(r => r.Name).ToArray();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }
    }
} 