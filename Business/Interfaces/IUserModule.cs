using Business.Models;
using System.Collections.Generic;
namespace Business.Interfaces;


public interface IUserModule
{
    bool CreateUser(User user);

    User GetUserById(int id);

    List<User> GetAllUsers();

    bool UpdateUser(User user);

    bool DeleteUser(int id);
}  