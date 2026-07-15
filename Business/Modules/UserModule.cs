using Business.Interfaces;
using Business.Models;
using DataAccess.Interfaces;
using DataAccess.Entities;
using Business.Exceptions;

namespace Business.Modules;

public class UserModule : IUserModule
{
    private readonly IUserRepository _userRepository;

    public UserModule(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool CreateUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrWhiteSpace(user.Name))
            throw new InvalidUserException("The user name is required.");

        if (string.IsNullOrWhiteSpace(user.LastName))
            throw new InvalidUserException("The user last name is required.");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new InvalidUserException("The user email is required.");

        if (user.Document <= 0)
            throw new InvalidUserException("The document must be greater than zero.");  

        UserEntity userEntity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            Document = user.Document
        };

        return _userRepository.CreateUser(userEntity);
    }

    public User GetUserById(int id)
    {
        UserEntity userEntity = _userRepository.GetUserById(id);

        if (userEntity == null)
            return null;

        return new User
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            LastName = userEntity.LastName,
            Email = userEntity.Email,
            Document = userEntity.Document
        };
    }

    public List<User> GetAllUsers()
    {
        List<UserEntity> userEntities = _userRepository.GetAllUsers();

        List<User> users = new List<User>();

        foreach (UserEntity userEntity in userEntities)
        {
            users.Add(new User
            {
                Id = userEntity.Id,
                Name = userEntity.Name,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                Document = userEntity.Document
            });
        }

        return users;
    }

    public bool UpdateUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        UserEntity userEntity = new UserEntity
        {
            Id = user.Id,
            Name = user.Name,
            LastName = user.LastName,
            Email = user.Email,
            Document = user.Document
        };

        return _userRepository.UpdateUser(userEntity);
    }

    public bool DeleteUser(int id)
    {
        return _userRepository.DeleteUser(id);
    }
}