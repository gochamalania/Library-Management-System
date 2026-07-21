using Core.Enums;
using Core.Models;
using Repository.Interfaces;

namespace Repository.Implementations;


public class TxtUserRepository : IUserRepository
{
    private readonly string _filePath;

    public TxtUserRepository(string filePath)
    {
        _filePath = filePath;

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

        if (!File.Exists(_filePath))
            return users;

        var lines = File.ReadAllLines(_filePath);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length < 4) continue;

            string id = parts[0].Trim();
            string username = parts[1].Trim();
            string password = parts[2].Trim();
            string roleStr = parts[3].Trim();

            // უსაფრთხო პარსინგი - თუ როლი ვერ იცნო, არ დაქრაშოს
            if (!Enum.TryParse<Role>(roleStr, true, out var role))
            {
                continue;
            }

            if (role == Role.Admin)
            {
                users.Add(new AdminUser(id, username, password));
            }
            else if (role == Role.Client)
            {
                decimal fines = 0;
                if (parts.Length > 4)
                {
                    decimal.TryParse(parts[4].Trim(), out fines);
                }
                users.Add(new ClientUser(id, username, password, fines));
            }
        }

        return users;
    }

    public User GetUserByUsername(string username)
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
            throw new Exception("მომხმარებელი ამ სახელით უკვე არსებობს!");
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
            string line = $"{user.Id}|{user.Username.Trim()}|{user.Password.Trim()}|{user.UserRole}";

            if (user is ClientUser client)
            {
                line += $"|{client.Fines}";
            }

            lines.Add(line);
        }

        File.WriteAllLines(_filePath, lines);
    }
}