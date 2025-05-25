using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class RoleApi
    {
        private readonly RoleRepository _roleRepository;
        
        public RoleApi()
        {
            var dbContext = new ApplicationDbContext();
            _roleRepository = new RoleRepository(dbContext);
        }
        
        protected List<Role> GetAllRolesFromDb()
        {
            return _roleRepository.GetAll().ToList();
        }
        
        protected Role GetRoleByIdFromDb(int id)
        {
            return _roleRepository.GetById(Guid.Parse(id.ToString()));
        }
        
        protected Role GetRoleByNameFromDb(string name)
        {
            return _roleRepository.GetByName(name);
        }
        
        protected bool SaveRoleToDb(Role role)
        {
            try
            {
                _roleRepository.Add(role);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 