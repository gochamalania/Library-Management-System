using Core.Enums;

namespace Core.Models;

public abstract class User
{
    public string Id { get; protected set; }
    public string Username { get; protected set; }
    public string Password { get; protected set; }
    public string Email { get; set; }
    public Role UserRole { get; protected set; }
    public bool IsEmailVerified { get; set; } = false;
    public string? VerificationCode { get; set; }

    public bool IsApproved { get; set; } = true;

    protected User() { }
    
    protected User
    (
        string id,
        string username,
        string password,
        string email,
        Role role,
        bool isApproved = true
        )
    {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        UserRole = role;
        IsEmailVerified = false;
        VerificationCode = null;
        IsApproved = isApproved;
    }

    public abstract void DisplayMenu();

}