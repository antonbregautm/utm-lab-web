using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class RoleBL : IRoleBL
    {
        private readonly RoleRepository _roleRepository;

        public RoleBL()
        {
            var dbContext = new ApplicationDbContext();
            _roleRepository = new RoleRepository(dbContext);
        }
        
        public List<Role> GetAllRoles()
        {
            return _roleRepository.GetAll().ToList();
        }
        
        public Role GetRoleById(Guid id)
        {
            return _roleRepository.GetById(id);
        }
        
        public Role GetRoleByName(string name)
        {
            return _roleRepository.GetByName(name);
        }
    }
} 