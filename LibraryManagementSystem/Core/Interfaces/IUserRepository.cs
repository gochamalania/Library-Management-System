using Core.Models;

namespace Core.Interfaces;

public interface IUserRepository
{
    List<User> GetAllUsers();
    User? GetUserByUsername(string username);
    User? GetUserByEmail(string email);
    void AddUser(User user);
    void UpdateUser(User user);
    void DeleteUser(User user);
}