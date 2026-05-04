using Microsoft.Data.Sqlite;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<DatabaseHelper>();

var app = builder.Build();

// Initialize database on startup
var db = app.Services.GetRequiredService<DatabaseHelper>();
db.Initialize();

// Serve static files (index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

// GET /employees — Retrieve all employees
app.MapGet("/employees", (DatabaseHelper db) =>
{
    var employees = db.GetAllEmployees();
    return Results.Json(employees);
});

// POST /employees — Add a new employee
app.MapPost("/employees", async (HttpRequest request, DatabaseHelper db) =>
{
    var body = await JsonSerializer.DeserializeAsync<EmployeeRequest>(request.Body);
    if (string.IsNullOrWhiteSpace(body?.Name))
        return Results.BadRequest(new { error = "Name is required" });

    db.AddEmployee(body.Name.Trim());
    return Results.Created("/employees", new { message = "Employee added successfully" });
});

app.Run("http://0.0.0.0:5000");

record EmployeeRequest(string Name);
