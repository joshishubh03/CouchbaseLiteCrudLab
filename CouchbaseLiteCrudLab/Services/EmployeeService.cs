using CouchbaseLiteCrudLab.Models;
using CouchbaseLiteCrudLab.Repositories;

namespace CouchbaseLiteCrudLab.Services;

public sealed class EmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public Employee? GetById(string id) => _repository.GetById(id);

    public IReadOnlyList<Employee> GetAll() => _repository.GetAll();

    public (bool Success, string Message) Create(Employee employee)
    {
        var error = Validate(employee);
        if (error is not null) return (false, error);

        return _repository.Create(employee)
            ? (true, "Employee created.")
            : (false, "An employee with that ID already exists.");
    }

    public (bool Success, string Message) Update(Employee employee)
    {
        var error = Validate(employee);
        if (error is not null) return (false, error);

        return _repository.Update(employee)
            ? (true, "Employee updated.")
            : (false, "Employee was not found.");
    }

    public (bool Success, string Message) Delete(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return (false, "Employee ID is required.");

        return _repository.Delete(id)
            ? (true, "Employee deleted.")
            : (false, "Employee was not found.");
    }

    private static string? Validate(Employee employee)
    {
        if (employee is null) return "Employee is required.";
        if (string.IsNullOrWhiteSpace(employee.Id)) return "Employee ID is required.";
        if (string.IsNullOrWhiteSpace(employee.FirstName)) return "First name is required.";
        if (string.IsNullOrWhiteSpace(employee.LastName)) return "Last name is required.";
        if (string.IsNullOrWhiteSpace(employee.Email)) return "Email is required.";
        if (!employee.Email.Contains('@')) return "Enter a valid email address.";
        if (string.IsNullOrWhiteSpace(employee.Department)) return "Department is required.";
        if (employee.Salary < 0) return "Salary cannot be negative.";
        return null;
    }
    public string? GetRawJsonById(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        return _repository.GetRawJsonById(id);
    }

}
