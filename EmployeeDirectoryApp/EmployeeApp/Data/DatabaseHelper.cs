using Microsoft.Data.Sqlite;
using EmployeeApp.Models;

public class DatabaseHelper
{
    private const string ConnectionString = "Data Source=employees.db";

    public void Initialize()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS employees (
                id   INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL
            )";
        cmd.ExecuteNonQuery();
    }

    public List<Employee> GetAllEmployees()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd  = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name FROM employees";
        var list = new List<Employee>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
            list.Add(new Employee { Id = reader.GetInt32(0), Name = reader.GetString(1) });
        return list;
    }

    public void AddEmployee(string name)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO employees (name) VALUES ($name)";
        cmd.Parameters.AddWithValue("$name", name);
        cmd.ExecuteNonQuery();
    }
}
