using System;
using System.Security.Cryptography;
using System.Text;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Core
{
    public class AuthApi
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;

        public AuthApi()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
        }
        
        protected User GetUserByEmailFromDb(string email)
        {
            return _userRepository.GetByEmail(email);
        }
        
        protected UserRepository GetUserRepository()
        {
            return _userRepository;
        }
        
        protected RoleRepository GetRoleRepository()
        {
            return _roleRepository;
        }
        
        protected string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        protected bool IsPasswordValid(string password, string storedHash)
        {
            string hashedPassword = CreatePasswordHash(password);
            return hashedPassword == storedHash;
        }
    }
} 