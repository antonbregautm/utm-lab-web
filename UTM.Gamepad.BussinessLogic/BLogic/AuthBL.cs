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
            return AuthenticateUserFromDb(email, password);
        }
        
        public bool CheckUserAuthorization(User user, string roleRequired)
        {
            return CheckUserAuthorizationFromDb(user, roleRequired);
        }

        public User CreateAccount(string fullName, string email, string password)
        {
            return CreateAccountInDb(fullName, email, password);
        }
        
        public User GetUserByEmail(string email)
        {
            return GetUserByEmailFromDb(email);
        }
        
        public AuthResultDto SignIn(string email, string password)
        {
            return SignInUser(email, password);
        }
        
        public AuthResultDto RegisterUser(string fullName, string email, string password)
        {
            return RegisterUserInDb(fullName, email, password);
        }
        
        public UserProfileDto GetUserProfile(string email)
        {
            return GetUserProfileFromDb(email);
        }
        
        public SignOutResultDto SignOut()
        {
            return SignOutUser();
        }
    }
} 