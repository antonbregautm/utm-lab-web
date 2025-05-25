using System;
using System.Collections.Generic;
using UTM.Gamepad.BussinessLogic.Core;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.BLogic
{
    public class RoleBL : RoleApi, IRoleBL
    {
        public RoleBL() : base() { }
        
        public Role GetRoleById(int id)
        {
            return GetRoleByIdFromDb(id);
        }
        
        public Role GetRoleById(Guid id)
        {
            // Для совместимости, пока передаем int внутри Guid
            return GetRoleByIdFromDb(Convert.ToInt32(id.ToString().Substring(0, 8), 16));
        }
        
        public Role GetRoleByName(string name)
        {
            return GetRoleByNameFromDb(name);
        }
        
        public List<Role> GetAllRoles()
        {
            return GetAllRolesFromDb();
        }
        
        public bool CreateRole(Role role)
        {
            if (role == null || string.IsNullOrEmpty(role.Name))
            {
                return false;
            }
            
            // Проверяем, существует ли уже роль с таким именем
            var existingRole = GetRoleByName(role.Name);
            if (existingRole != null)
            {
                return false;
            }
            
            return SaveRoleToDb(role);
        }
    }
} 