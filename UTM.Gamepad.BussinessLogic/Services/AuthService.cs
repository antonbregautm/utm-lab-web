using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;

        public AuthService()
        {
            var dbContext = new ApplicationDbContext();
            _userRepository = new UserRepository(dbContext);
            _roleRepository = new RoleRepository(dbContext);
        }

        public User AuthenticateUser(string email, string password)
        {
            var user = _userRepository.GetByEmail(email);
            if (user != null && IsPasswordValid(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
        
        public bool CheckUserAuthorization(User user, string roleRequired)
        {
            return user.Role != null && user.Role.Name == roleRequired;
        }

        public User CreateAccount(string fullName, string email, string password)
        {
            if (_userRepository.GetByEmail(email) != null)
            {
                return null;
            }
            
            string passwordHash = CreatePasswordHash(password);

            var defaultRole = _roleRepository.GetByName("User");
            
            if (defaultRole == null)
            {
                defaultRole = new Role { Name = "User", Description = "Обычный пользователь системы" };
                _roleRepository.Add(defaultRole);
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                RoleId = defaultRole.Id,
                Role = defaultRole  
            };
            
            return _userRepository.Add(newUser);
        }
        
        public static string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static bool IsPasswordValid(string password, string storedHash)
        {
            string hashedPassword = CreatePasswordHash(password);
            return hashedPassword == storedHash;
        }
        
        public User GetUserByEmail(string email)
        {
            return _userRepository.GetByEmail(email);
        }
    }
}