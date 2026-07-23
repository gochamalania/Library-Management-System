using System.Text.Json;
using Core.Enums;
using Core.Interfaces;
using Core.Models;

namespace Repository.Implementations;

public class JsonUserRepository : IUserRepository
{
    private readonly string _filePath;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true, // Saves nicely formatted JSON
    };

    public JsonUserRepository(string filePath)
    {
        _filePath = filePath;
        
        string? directory = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // If the file does not exist or is empty, we write an empty JSON array "[]"
        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public List<User> GetAllUsers()
    {
        if (!File.Exists(_filePath))
            return new List<User>();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<User>();

        try
        {
            var dtos = JsonSerializer.Deserialize<List<UserDto>>(json, _jsonOptions) ?? new List<UserDto>();
            var users = new List<User>();

            foreach (var dto in dtos)
            {
                if (dto.UserRole == Role.Admin)
                {
                    users.Add(new AdminUser(dto.Id, dto.Username, dto.Password));
                }
                else if (dto.UserRole == Role.Client)
                {
                    users.Add(new ClientUser(dto.Id, dto.Username, dto.Password, dto.Fines));
                }
            }

            return users;
        }

        catch
        {
            return new List<User>();
        }
    }

    public User? GetUserByUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return null;
        
        string cleanUsername = username.Trim();
        return GetAllUsers().FirstOrDefault(u => u.Username.Equals(cleanUsername, StringComparison.OrdinalIgnoreCase));
    }

    public void AddUser(User user)
    {
        var users = GetAllUsers();
        if (users.Any(u => u.Username.Equals(user.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("User with that username already exists");
        }
        users.Add(user);
        SaveAll(users);
    }

    public void UpdateUser(User user)
    {
        var users = GetAllUsers();
        var index = users.FindIndex(u => u.Id == user.Id);

        if (index != -1)
        {
            users[index] = user;
            SaveAll(users);
        }
    }
    
    private void SaveAll(List<User> users)
        {
            var dtos = new List<UserDto>();
    
            foreach (var user in users)
            {
                var dto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password,
                    UserRole = user.UserRole,
                    Fines = (user is ClientUser client) ? client.Fines : 0
                };
                dtos.Add(dto);
            }
    
            var json = JsonSerializer.Serialize(dtos, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
    
    // Helper DTO class for working with JSON
    private class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role UserRole  { get; set; }
        public decimal Fines { get; set; }
    }
}