using System.Web;
using UTM.Gamepad.Domain;
using UTM.Gamepad.Domain.DTOs;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IAuthBL
    {
        User AuthenticateUser(string email, string password);
        bool CheckUserAuthorization(User user, string roleRequired);
        User CreateAccount(string fullName, string email, string password);
        User GetUserByEmail(string email);
        AuthResultDto SignIn(string email, string password);
        AuthResultDto RegisterUser(string fullName, string email, string password);
        UserProfileDto GetUserProfile(string email);
        SignOutResultDto SignOut();
    }
} 