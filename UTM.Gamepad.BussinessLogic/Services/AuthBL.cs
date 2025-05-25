using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using UTM.Gamepad.BussinessLogic.Services.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.Services
{
    public class AuthBL : IAuthBL
    {
        private readonly UserRepository _userRepository;
        private readonly RoleRepository _roleRepository;

        public AuthBL()
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
        
        private static string CreatePasswordHash(string password)
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
        
        // Новые методы с DTO
        
        public AuthResultDto SignIn(string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Неверный email или пароль"
            };
            
            var user = AuthenticateUser(email, password);
            if (user != null)
            {
                result.IsSuccess = true;
                result.ErrorMessage = null;
                result.UserId = user.Id;
                result.Email = user.Email;
                result.UserRole = user.Role?.Name ?? "User";
                result.AuthCookie = CreateAuthCookie(user);
            }
            
            return result;
        }
        
        public AuthResultDto RegisterUser(string fullName, string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Пользователь с таким email уже существует"
            };
            
            var user = CreateAccount(fullName, email, password);
            if (user != null)
            {
                result.IsSuccess = true;
                result.ErrorMessage = null;
                result.UserId = user.Id;
                result.Email = user.Email;
                result.UserRole = user.Role?.Name ?? "User";
                result.AuthCookie = CreateAuthCookie(user);
            }
            
            return result;
        }
        
        public UserProfileDto GetUserProfile(string email)
        {
            var user = GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }
            
            return new UserProfileDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role?.Name ?? "Пользователь"
            };
        }
        
        public SignOutResultDto SignOut()
        {
            FormsAuthentication.SignOut();
            
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
            {
                Expires = DateTime.Now.AddYears(-1)
            };
            
            return new SignOutResultDto
            {
                AuthCookie = authCookie
            };
        }
        
        // Вспомогательный метод для создания куки аутентификации
        private HttpCookie CreateAuthCookie(User user)
        {
            string roleName = user.Role?.Name ?? "User";
            
            var ticket = new FormsAuthenticationTicket(
                1, // ticket version
                user.Email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30), // expiration
                true, // persistent cookie
                roleName, // user data
                "/");

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            return new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        }
    }
} 