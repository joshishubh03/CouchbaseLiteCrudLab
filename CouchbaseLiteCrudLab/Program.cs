using Couchbase.Lite;
using CouchbaseLiteCrudLab;
using CouchbaseLiteCrudLab.Repositories;
using CouchbaseLiteCrudLab.Services;

const string databaseName = "employeecrudlab";

var builder = WebApplication.CreateBuilder(args);

// Keep the same database name and default configuration as Labs 1–3.
var database = new Database(databaseName);
var collection = database.GetDefaultCollection();

IEmployeeRepository repository =
    new CouchbaseEmployeeRepository(collection);
var employeeService = new EmployeeService(repository);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

await SyncToCapella.RunOnceAsync(
    database,
    collection,
    "https://gralrjszvlfvptvq.apps.cloud.couchbase.com:4984/employee-sync-endpoint",
    "test",
    "w06Eui2Taz@shubham");

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Running",
    database = databaseName
}));

app.MapGet("/api/employees", () =>
    Results.Ok(employeeService.GetAll()));

app.MapGet("/api/employees/{id}", (string id) =>
{
    var employee = employeeService.GetById(id);
    return employee is null
        ? Results.NotFound(new { message = "Employee not found." })
        : Results.Ok(employee);
});

app.MapGet("/api/documents/{id}/json", (string id) =>
{
    var json = employeeService.GetRawJsonById(id);
    return json is null
        ? Results.NotFound(new { message = "Document not found." })
        : Results.Text(json, "application/json");
});

app.MapGet("/api/summary", () =>
{
    var employees = employeeService.GetAll();
    var departmentCount = employees
        .Select(e => e.Department)
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Count();

    return Results.Ok(new
    {
        totalEmployees = employees.Count,
        departmentCount
    });
});

app.Lifetime.ApplicationStopping.Register(database.Dispose);
app.Run();
