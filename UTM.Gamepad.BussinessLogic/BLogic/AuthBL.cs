using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using UTM.Gamepad.BussinessLogic.Core;
using UTM.Gamepad.BussinessLogic.Interfaces;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;
using UTM.Gamepad.Infrastructure;
using UTM.Gamepad.Infrastructure.Repository;

namespace UTM.Gamepad.BussinessLogic.BLogic
{
    public class AuthBL : AuthApi, IAuthBL
    {
        public AuthBL() : base() { }
        
        public User AuthenticateUser(string email, string password)
        {
            var user = GetUserByEmailFromDb(email);
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
        
        private static new string CreatePasswordHash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private static new bool IsPasswordValid(string password, string storedHash)
        {
            string hashedPassword = CreatePasswordHash(password);
            return hashedPassword == storedHash;
        }
        
        public User GetUserByEmail(string email)
        {
            return GetUserByEmailFromDb(email);
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
                result.UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16);
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
                result.UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16);
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
                UserId = Convert.ToInt32(user.Id.ToString().Substring(0, 8), 16),
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