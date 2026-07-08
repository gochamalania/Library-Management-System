using Core.Models;
// using System.Collections.Generic;

namespace Repository.Interfaces;

public interface IUserRepository
{
    List<User> GetAllUsers();
    User GetUserByUsername(string username);
    void AddUser(User user);
    void UpdateUser(User user);
}