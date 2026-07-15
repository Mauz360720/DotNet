using Microsoft.Data.Sqlite;

namespace DataAccess.Context;

public class DatabaseContext
{
    private readonly SqliteConnection _connection;

    public DatabaseContext()
    {
        _connection = new SqliteConnection("Data Source=school.db");
        _connection.Open();

        CreateDatabase();
    }

    private void CreateDatabase()
    {
        var command = _connection.CreateCommand();

        command.CommandText =
            @"
        CREATE TABLE IF NOT EXISTS Users
        (
            Id INTEGER PRIMARY KEY,
            Name TEXT NOT NULL,
            LastName TEXT NOT NULL,
            Email TEXT NOT NULL,
            Document INTEGER NOT NULL
        );
        ";

        command.ExecuteNonQuery();
    }

    public SqliteConnection GetConnection()
    {
        return _connection;
    }
}