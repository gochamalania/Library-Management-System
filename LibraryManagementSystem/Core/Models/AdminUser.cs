using Core.Enums;

namespace Core.Models;

public class AdminUser : User
{
    public AdminUser(string id, string username, string password) : base(id, username, password, Role.Admin) 
    {
        
    }
    
    public override void DisplayMenu()
    {
        // Implementation will be done in the UI layer
    }
}