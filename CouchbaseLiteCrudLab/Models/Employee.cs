namespace CouchbaseLiteCrudLab.Models;

public sealed class Employee
{
    public required string Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Department { get; init; }
    public double Salary { get; init; }
    public string? CreatedAtUtc { get; init; }
    public string? UpdatedAtUtc { get; init; }

    public string FullName => $"{FirstName} {LastName}";
}
