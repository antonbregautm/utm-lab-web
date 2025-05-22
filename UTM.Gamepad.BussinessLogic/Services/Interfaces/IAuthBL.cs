using UTM.Gamepad.Domain;

namespace UTM.Gamepad.BussinessLogic.Services.Interfaces
{
    public interface IAuthBL
    {
        User AuthenticateUser(string email, string password);
        bool CheckUserAuthorization(User user, string roleRequired);
        User CreateAccount(string fullName, string email, string password);
        User GetUserByEmail(string email);
    }
} 