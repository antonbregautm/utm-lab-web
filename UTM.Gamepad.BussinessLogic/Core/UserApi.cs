using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class UserApi
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;
        
        public UserApi()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
        }
        
        protected List<User> GetAllUsersFromDb()
        {
            return _userRepository.GetAll().ToList();
        }
        
        protected User GetUserByIdFromDb(Guid id)
        {
            return _userRepository.GetById(id);
        }
        
        protected User GetUserByEmailFromDb(string email)
        {
            return _userRepository.GetByEmail(email);
        }
        
        protected bool SaveUserToDb(User user)
        {
            try
            {
                _userRepository.Add(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool UpdateUserInDb(User user)
        {
            try
            {
                _userRepository.Update(user);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected bool DeleteUserFromDb(Guid id)
        {
            try
            {
                _userRepository.Delete(id);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        protected string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        
        protected Role GetRoleById(int id)
        {
            return _roleRepository.GetById(Guid.Parse(id.ToString()));
        }
    }
} 