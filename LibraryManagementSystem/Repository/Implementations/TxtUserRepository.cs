using Core.Enums;
using Core.Models;
using Repository.Interfaces;

namespace Repository.Implementations;

public class TxtUserRepository : IUserRepository
{
    private readonly string _filePath;
    
    // pass the file path from the constructor
    public TxtUserRepository(string filePath)
    {
        _filePath = filePath;
        
        // If the folder or file does not exist, we automatically create it.
        string directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Close();
        }
        
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();
        var lines = File.ReadAllLines(_filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length < 4) continue;
            
            string id = parts[0];
            string username = parts[1];
            string password = parts[2];
            Role role = Enum.Parse<Role>(parts[3]);

            if (role == Role.Admin)
            {
                users.Add(new AdminUser(id, username, password));
            } 
            else if(role == Role.Client)
            {
                decimal fines = parts.Length > 4 ? decimal.Parse(parts[4]) : 0;
                users.Add(new ClientUser(id, username, password, fines));
            }
        }
        return users;
    }

    public User GetUserByUsername(string username)
    {
        return GetAllUsers().FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    public void AddUser(User user)
    {
        var users = GetAllUsers();
        
        // Check if such user already exists
        if (users.Any(u => u.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("User already exists");
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
        var lines = new List<string>();

        foreach (var user in users)
        {
            string line = $"{user.Id} {user.Username} {user.Password} {user.UserRole}";
            
            // If it is a client, we also add the fine at the end.
            if (user is ClientUser client)
            {
                line += $"|{client.Fines}";
            }
            lines.Add(line);
        }
        File.WriteAllLines(_filePath, lines);
    }
}