using Application.Interfaces;
using Core.Enums;
using Core.Models;
using Core.Interfaces;
using System;
using System.Linq;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    
    // Encapsulation: CurrentUser can be read from outside, modified - only in this class
    public User CurrentUser { get; private set; }
    
    // Dependency Injection: We get the repository from the constructor
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool Register(string username, string password, Role role)
    {
        // 1. Data validation
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("User name and password cannot be empty");
        }

        if (password.Length < 4)
        {
            throw new ArgumentException("Password must consist of at least 4 characters.");
        }
        
        //2. Check: Does such a user already exist?
        var existingUser = _userRepository.GetUserByUsername(username);
        if (existingUser != null)
        {
            throw new InvalidOperationException("A user with this name already exists.");
        }
        
        //3. Generate a unique ID (e.g. 1001, 1002...)
        string newId = GenerateUniqueId();
        
        // 4. Create the corresponding role object
        User newUser;
        if (role == Role.Admin)
        {
            newUser = new AdminUser(newId, username, password);
        }
        else
        {
            newUser = new ClientUser(newId, username, password);
        }
        
        // 5. Save to file database
        _userRepository.AddUser(newUser);
        return true;
    }

    public bool Login(string username, string password)
    {
        //trim
        string cleanUsername = username.Trim();
        string cleanPassword = password.Trim();
        
        var user = _userRepository.GetUserByUsername(username);
        
        // If user not found or password is incorrect
        if (user == null || user.Password != password)
        {
            return false;
        }
        
        // Authorization successful — we remember the current user
        CurrentUser = user;
        return true;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
    
    // Helper method to automatically generate a unique ID
    private string GenerateUniqueId()
    {
        var users = _userRepository.GetAllUsers();
        if (!users.Any()) return "1001";
        
        // Find the largest ID and add 1 to it
        int maxId = users.Select(u => int.TryParse(u.Id, out int id) ? id : 1000).Max();
        return (maxId + 1).ToString();
    }
}