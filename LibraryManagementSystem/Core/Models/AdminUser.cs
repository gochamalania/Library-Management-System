using Core.Enums;

namespace Core.Models;

public class AdminUser : User
{
    public AdminUser() { }
    
    public AdminUser(string id, string username, string password, string email, Role role = Role.Admin) 
        : base(id, username, password, email, role) 
    {
        
    }
    
    public override void DisplayMenu()
    {
        // Implementation will be done in the UI layer
    }
}