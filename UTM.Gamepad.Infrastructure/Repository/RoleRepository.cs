using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Infrastructure.Repository
{
    public class RoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Role GetById(Guid id)
        {
            return _dbContext.Roles.Find(id);
        }

        public Role GetByName(string name)
        {
            return _dbContext.Roles.FirstOrDefault(r => r.Name == name);
        }

        public IEnumerable<Role> GetAll()
        {
            return _dbContext.Roles.ToList();
        }

        public Role Add(Role role)
        {
            _dbContext.Roles.Add(role);
            _dbContext.SaveChanges();
            return role;
        }

        public Role Update(Role role)
        {
            _dbContext.Entry(role).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return role;
        }

        public void Delete(Guid id)
        {
            var role = _dbContext.Roles.Find(id);
            if (role != null)
            {
                _dbContext.Roles.Remove(role);
                _dbContext.SaveChanges();
            }
        }
    }
} 