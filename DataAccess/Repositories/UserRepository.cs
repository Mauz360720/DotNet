using Microsoft.Data.Sqlite;
using DataAccess.Context;
using DataAccess.Entities;
using DataAccess.Interfaces;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DatabaseContext _databaseContext;

    public UserRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }
    public bool CreateUser(UserEntity user)
    {
        SqliteConnection connection = _databaseContext.GetConnection();

        var command = connection.CreateCommand();

        command.CommandText =
            @"
        INSERT INTO Users
        (Id, Name, LastName, Email, Document)
        VALUES
        ($id, $name, $lastName, $email, $document);
    ";

        command.Parameters.AddWithValue("$id", user.Id);
        command.Parameters.AddWithValue("$name", user.Name);
        command.Parameters.AddWithValue("$lastName", user.LastName);
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$document", user.Document);

        int rows = command.ExecuteNonQuery();

        return rows > 0;
    }

    public UserEntity GetUserById(int id)
    {
        SqliteConnection connection = _databaseContext.GetConnection();

        var command = connection.CreateCommand();

        command.CommandText =
            @"
        SELECT Id, Name, LastName, Email, Document
        FROM Users
        WHERE Id = $id;
    ";

        command.Parameters.AddWithValue("$id", id);

        var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new UserEntity
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            Document = reader.GetInt32(4)
        };   
    }

    public List<UserEntity> GetAllUsers()
    {
        SqliteConnection connection = _databaseContext.GetConnection();

        var command = connection.CreateCommand();

        command.CommandText =
            @"
        SELECT Id, Name, LastName, Email, Document
        FROM Users;
    ";

        var reader = command.ExecuteReader();

        List<UserEntity> users = new List<UserEntity>();

        while (reader.Read())
        {
            users.Add(new UserEntity
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                LastName = reader.GetString(2),
                Email = reader.GetString(3),
                Document = reader.GetInt32(4)
            });
        }

        return users;
    }

    public bool UpdateUser(UserEntity user)
    {
        SqliteConnection connection = _databaseContext.GetConnection();

        var command = connection.CreateCommand();

        command.CommandText =
            @"
        UPDATE Users
        SET
            Name = $name,
            LastName = $lastName,
            Email = $email,
            Document = $document
        WHERE Id = $id;
    ";

        command.Parameters.AddWithValue("$id", user.Id);
        command.Parameters.AddWithValue("$name", user.Name);
        command.Parameters.AddWithValue("$lastName", user.LastName);
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$document", user.Document);

        int rows = command.ExecuteNonQuery();

        return rows > 0;
    }

    public bool DeleteUser(int id)
    {
        SqliteConnection connection = _databaseContext.GetConnection();

        var command = connection.CreateCommand();

        command.CommandText =
            @"
        DELETE FROM Users
        WHERE Id = $id;
    ";

        command.Parameters.AddWithValue("$id", id);

        int rows = command.ExecuteNonQuery();

        return rows > 0;
        
    } 
    