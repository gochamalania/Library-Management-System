using Application.Interfaces;
using Core.Enums;
using Core.Models;
using Core.Interfaces;
using System.Text.RegularExpressions;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggerService _logger;
    
    public User? CurrentUser { get; private set; }
    
    public AuthService(IUserRepository userRepository, ILoggerService logger)
    {
        _userRepository = userRepository;
        _logger = logger;

        SeedSuperAdmin();
    }

    private void SeedSuperAdmin()
    {
        var users = _userRepository.GetAllUsers();
        bool hasSuperAdmin = users.Any(u => u.UserRole == Role.SuperAdmin);

        if (!hasSuperAdmin)
        {
            string id = GenerateUniqueId();
            var superAdmin = new AdminUser
            (
                id,
                "superadmin",
                "SuperAdmin123!",
                "superadmin@library.ge",
                Role.SuperAdmin
            )
            {
                IsEmailVerified = true,
                IsApproved = true 
            };
            
            _userRepository.AddUser(superAdmin);
            _logger.LogInfo($"User '{superAdmin.Username}' registered successfully");
        }
    }

    public bool Register(string username, string password, string email, Role role)
    {
        string cleanUsername = username?.Trim() ?? "";
        string cleanPassword = password?.Trim() ?? "";
        string cleanEmail = email?.Trim() ?? "";
        
        if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(cleanPassword) || string.IsNullOrWhiteSpace(cleanEmail))
        {
            _logger.LogWarning("Attempted registration with an empty username or password.");
            throw new ArgumentException("User name and password cannot be empty");
        }
        
        if (!IsValidEmail(cleanEmail))
        {
            _logger.LogWarning($"Invalid Email Format: {cleanEmail}");
            throw new ArgumentException("Invalid Email Format!");
        }

        if (cleanPassword.Length < 4)
        {
            _logger.LogWarning($"Attempted registration with a length of 4 characters, USER {cleanUsername}");
            throw new ArgumentException("Password must consist of at least 4 characters.");
        }
        
        if (_userRepository.GetUserByUsername(cleanUsername) != null)
        {
            _logger.LogWarning($"Registration attempt: User '{cleanUsername}' already exists");
            throw new InvalidOperationException("A user with this name already exists.");
        }
        
        if (_userRepository.GetUserByEmail(cleanEmail) != null)
        {
            _logger.LogWarning($"Registration attempt: email '{cleanEmail}' already exists");
            throw new InvalidOperationException("A user with this email already exists.");
        }
        
        string newId = GenerateUniqueId();
        string verificationCode = new Random().Next(100000, 999999).ToString();
        
        User newUser;
        if (role == Role.Admin)
        {
            newUser = new AdminUser(newId, cleanUsername, cleanPassword, cleanEmail, Role.Admin);
            newUser.IsApproved = false; // An admin who came with a regular registration needs an approve!
        }
        else
        {
            newUser = new ClientUser(newId, cleanUsername, cleanPassword, cleanEmail);
            newUser.IsApproved = true; // The client does not need an approve.
        }
        
        newUser.VerificationCode = verificationCode;
        
        _userRepository.AddUser(newUser);
        _logger.LogInfo($"New user registered successfully: '{cleanUsername}' (Email: {cleanEmail}), verification code: {verificationCode}");
        
        return true;
    }

    // Direct admin registration by super-admin
    public void RegisterAdmin(string username, string password)
    {
        string cleanUsername = username?.Trim() ?? "";
        string cleanPassword = password?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(cleanUsername) || string.IsNullOrWhiteSpace(cleanPassword))
        {
            throw new ArgumentException("Username and Password cannot be empty.");
        }

        if (_userRepository.GetUserByUsername(cleanUsername) != null)
        {
            throw new InvalidOperationException("A user with this username already exists.");
        }

        string newId = GenerateUniqueId();
        string email = $"{cleanUsername.ToLower()}@admin.library.ge";

        var admin = new AdminUser(newId, cleanUsername, cleanPassword, email, Role.Admin)
        {
            IsEmailVerified = true,
            IsApproved = true
        };

        _userRepository.AddUser(admin);
        _logger.LogInfo($"SuperAdmin directly created new Admin: '{cleanUsername}'");
    }

    public bool Login(string identifier, string password)
    {
        string cleanIdentifier = identifier?.Trim() ?? "";
        string cleanPassword = password?.Trim() ?? "";
        
        var user = _userRepository.GetUserByUsername(cleanIdentifier)
            ?? _userRepository.GetUserByEmail(cleanIdentifier);

        if (user == null || user.Password != cleanPassword)
        {
            _logger.LogWarning($"Unsuccessful authorization attempt: '{cleanIdentifier}'");
            return false;
        }

        
        if (!user.IsApproved)
        {
            _logger.LogWarning($"Unapproved admin '{user.Username}' attempted login.");
            throw new InvalidOperationException("Your Admin account is pending SuperAdmin approval!");
        }
        
        CurrentUser = user;
        _logger.LogInfo($"User '{user.Username}' ({user.Email}) Logged in successfully");
        return true;
    }

    
    public List<User> GetPendingAdmins()
    {
        return _userRepository.GetAllUsers()
            .Where(u => u.UserRole == Role.Admin && !u.IsApproved)
            .ToList();
    }

    
    public void ApproveAdmin(string userId)
    {
        var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == userId);
        
        if (user == null)
        {
            throw new InvalidOperationException("User not found!");
        }

        user.IsApproved = true;
        _userRepository.UpdateUser(user);
        _logger.LogInfo($"Admin '{user.Username}' (ID: {userId}) was approved by SuperAdmin.");
    }

    public bool VerifyEmail(string email, string code)
    {
        var user = _userRepository.GetUserByEmail(email);

        if (user == null)
        {
            _logger.LogWarning($"Attempting verification on an non-existing email: {email}");
            return false;
        }

        if (user.IsEmailVerified)
        {
            return true;
        }

        if (user.VerificationCode == code.Trim())
        {
            user.IsEmailVerified = true;
            user.VerificationCode = null;
            _userRepository.UpdateUser(user);
            
            _logger.LogInfo($"Email '{email}' Successfully verified.");
            return true;
        }
        
        _logger.LogWarning($"Invalid verification code for email: {email}");
        return false;
    }

    public string ResendVerificationCode(string email)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null) throw new Exception("User not found!");
        
        string newCode = new Random().Next(100000, 999999).ToString();
        user.VerificationCode = newCode;
        _userRepository.UpdateUser(user);
        
        _logger.LogInfo($"A new verification code has been sent to your email: {email} (code: {newCode})");
        return newCode;
    }

    public void Logout()
    {
        if (CurrentUser != null)
        {
            _logger.LogInfo($"User '{CurrentUser.Username}' logged out.");
        }
        CurrentUser = null;
    }
    
    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    private string GenerateUniqueId()
    {
        var users = _userRepository.GetAllUsers();
        if (!users.Any()) return "1001";

        int maxId = users.Select(u => int.TryParse(u.Id, out int id) ? id : 1000).Max();
        return (maxId + 1).ToString();
    }
}