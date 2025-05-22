using System;
using System.Collections.Generic;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IRoleBL
    {
        List<Role> GetAllRoles();
        Role GetRoleById(Guid id);
        Role GetRoleByName(string name);
    }
} 