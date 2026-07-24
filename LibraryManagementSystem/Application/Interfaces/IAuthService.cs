using Core.Models;
using Core.Enums;

namespace Application.Interfaces;

public interface IAuthService
{
    User? CurrentUser { get; }
    
    //registration
    void RegisterAdmin(string username, string password);
    bool Register(string username, string password, string email, Role role);
    
    //login identifier = username or email
    bool Login(string identifier, string password);
    bool VerifyEmail(string email, string code);
    string ResendVerificationCode(string email);
    
    void Logout();

    List<User> GetPendingAdmins();
    void ApproveAdmin(string userId);

}