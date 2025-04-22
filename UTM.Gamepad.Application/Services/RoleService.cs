using System;
using System.Collections.Generic;
using System.Linq;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.Application.Services
{
    public class RoleService
    {
        private readonly RoleRepository _roleRepository;
        private readonly UserRepository _userRepository;

        public RoleService()
        {
            var dbContext = new ApplicationDbContext();
            _roleRepository = new RoleRepository(dbContext);
            _userRepository = new UserRepository(dbContext);
        }
        
        public List<Role> GetAllRoles()
        {
            return _roleRepository.GetAll().ToList();
        }
        
        public Role GetRoleById(Guid id)
        {
            return _roleRepository.GetById(id);
        }
        
        public Role CreateRole(string name, string description)
        {
            var role = new Role
            {
                Name = name,
                Description = description
            };
            
            return _roleRepository.Add(role);
        }
        
        public bool UpdateRole(Guid id, string name, string description)
        {
            var role = _roleRepository.GetById(id);
            if (role == null)
                return false;
            
            role.Name = name;
            role.Description = description;
            
            _roleRepository.Update(role);
            return true;
        }
        
        public bool DeleteRole(Guid id)
        {
            var role = _roleRepository.GetById(id);
            if (role == null)
                return false;
            
            _roleRepository.Delete(id);
            return true;
        }
        
        public bool AssignRoleToUser(Guid userId, Guid roleId)
        {
            var user = _userRepository.GetById(userId);
            var role = _roleRepository.GetById(roleId);
            
            if (user == null || role == null)
                return false;
            
            user.RoleId = role.Id;
            _userRepository.Update(user);
            return true;
        }
        
        public void InitializeDefaultRoles()
        {
            try
            {
                if (!_roleRepository.GetAll().Any())
                {
                    var roles = new List<Role>
                    {
                        new Role { Name = "Admin", Description = "Администратор системы с полными правами" },
                        new Role { Name = "User", Description = "Обычный пользователь системы" },
                        new Role { Name = "Manager", Description = "Менеджер с расширенными правами" }
                    };
                    
                    foreach (var role in roles)
                    {
                        _roleRepository.Add(role);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка при инициализации ролей: {ex.Message}");
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
            }
        }
    }
} 