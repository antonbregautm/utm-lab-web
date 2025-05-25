using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Interfaces
{
    public interface IRoleBL
    {
        Role GetRoleById(int id);
        Role GetRoleById(Guid id);
        Role GetRoleByName(string name);
        List<Role> GetAllRoles();
        bool CreateRole(Role role);
    }
} 