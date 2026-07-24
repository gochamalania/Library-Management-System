using Core.Enums;

namespace Core.Models;

public class ClientUser : User
{
    public decimal Fines { get; private set; }
    
    public ClientUser() { }
    
    public ClientUser(
        string id, string username, string password, string email, decimal fines = 0
        ) : base(id, username, password, email, Role.Client)
    {
        Fines = fines;
    }

    public void AddFine(decimal amount)
    {
        if (amount > 0)
            Fines += amount;
    }

    public void PayFine(decimal amount)
    {
        if (amount > 0 && amount <= Fines)
            Fines -= amount;
        else if (amount > Fines)
            Fines = 0;
    }

    public override void DisplayMenu()
    {
        // Implementation will be done in the UI layer
    }
}