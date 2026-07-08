using Core.Enums;

namespace Core.Models;

public abstract class User
{
    public string Id { get; protected set; }
    public string Username { get; protected set; }
    public string Password { get; protected set; }
    public Role UserRole { get; protected set; }

    protected User(
        string id,
        string username,
        string password,
        Role role)
    {
        Id = id;
        Username = username;
        Password = password;
        UserRole = role;
    }

    public abstract void DisplayMenu();

}