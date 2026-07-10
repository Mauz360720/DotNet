using DataAccess.Entities;

namespace DataAccess.Interfaces;

public interface IUserRepository
{
    bool CreateUser (UserEntity user);
    
    UserEntity GetUserById (int id);
    
    List<UserEntity> GetAllUsers ();
    
    bool UpdateUser (UserEntity user);
    
    bool DeleteUser (int id);
}