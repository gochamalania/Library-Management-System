using Core.Models;
using Core.Enums;

namespace Application.Interfaces;

public interface IAuthService
{
    User CurrentUser { get; }
    
    //registration
    bool Register(string username, string password, Role role);
    
    //login
    bool Login(string username, string password);
    
    void Logout();
    
}