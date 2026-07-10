using DataAccess.Entities;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    public bool CreateUser(UserEntity user)
    {
        throw new NotImplementedException();
    }

    public UserEntity GetUserById(int id)
    {
        throw new NotImplementedException();
    }

    public List<UserEntity> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public bool UpdateUser(UserEntity user)
    {
        throw new NotImplementedException();
    }

    public bool DeleteUser(int id)
    {
        throw new NotImplementedException();
    }
}