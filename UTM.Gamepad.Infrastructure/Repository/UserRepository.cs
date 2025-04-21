using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using UTM.Gamepad.Domain;

namespace UTM.Gamepad.Infrastructure.Repository
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetById(Guid id)
        {
            return _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Id == id);
        }

        public User GetByEmail(string email)
        {
            return _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == email);
        }

        public IEnumerable<User> GetAll()
        {
            return _dbContext.Users
                .Include(u => u.Role)
                .ToList();
        }

        public User Add(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user;
        }

        public User Update(User user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return user;
        }

        public void Delete(Guid id)
        {
            var user = _dbContext.Users.Find(id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                _dbContext.SaveChanges();
            }
        }
    }
} 