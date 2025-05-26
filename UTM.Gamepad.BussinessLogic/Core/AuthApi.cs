using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;
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
        
        protected User AuthenticateUserFromDb(string email, string password)
        {
            var user = GetUserByEmailFromDb(email);
            if (user != null && IsPasswordValid(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }
        
        protected bool CheckUserAuthorizationFromDb(User user, string roleRequired)
        {
            return user.Role != null && user.Role.Name == roleRequired;
        }
        
        protected User CreateAccountInDb(string fullName, string email, string password)
        {
            if (GetUserByEmailFromDb(email) != null)
            {
                return null;
            }
            
            string passwordHash = CreatePasswordHash(password);

            var defaultRole = GetRoleRepository().GetByName("User");
            
            if (defaultRole == null)
            {
                defaultRole = new Role { Name = "User", Description = "Обычный пользователь системы" };
                GetRoleRepository().Add(defaultRole);
            }

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                RoleId = defaultRole.Id,
                Role = defaultRole  
            };
            
            return GetUserRepository().Add(newUser);
        }
        
        protected AuthResultDto SignInUser(string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Неверный email или пароль"
            };
            
            var user = AuthenticateUserFromDb(email, password);
            if (user != null)
            {
                result.IsSuccess = true;
                result.ErrorMessage = null;
                result.UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16);
                result.Email = user.Email;
                result.UserRole = user.Role?.Name ?? "User";
                result.AuthCookie = CreateAuthCookieInDb(user);
            }
            
            return result;
        }
        
        protected AuthResultDto RegisterUserInDb(string fullName, string email, string password)
        {
            var result = new AuthResultDto
            {
                IsSuccess = false,
                ErrorMessage = "Пользователь с таким email уже существует"
            };
            
            var user = CreateAccountInDb(fullName, email, password);
            if (user != null)
            {
                result.IsSuccess = true;
                result.ErrorMessage = null;
                result.UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16);
                result.Email = user.Email;
                result.UserRole = user.Role?.Name ?? "User";
                result.AuthCookie = CreateAuthCookieInDb(user);
            }
            
            return result;
        }
        
        protected UserProfileDto GetUserProfileFromDb(string email)
        {
            var user = GetUserByEmailFromDb(email);
            if (user == null)
            {
                return null;
            }
            
            return new UserProfileDto
            {
                UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16),
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role?.Name ?? "Пользователь"
            };
        }
        
        protected SignOutResultDto SignOutUser()
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
        protected HttpCookie CreateAuthCookieInDb(User user)
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