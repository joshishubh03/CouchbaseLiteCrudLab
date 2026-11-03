using Couchbase.Lite;
using CouchbaseLiteCrudLab.Models;

namespace CouchbaseLiteCrudLab.Repositories;

public sealed class CouchbaseEmployeeRepository : IEmployeeRepository
{
    private readonly Collection _collection;

    public CouchbaseEmployeeRepository(Collection collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public Employee? GetById(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var doc = _collection.GetDocument(id);
        if (doc is null) return null;

        return Map(doc);
    }

    public IReadOnlyList<Employee> GetAll()
    {
        const string sql = @"
            SELECT META().id AS id, firstName, lastName, email,
                   department, salary, createdAtUtc, updatedAtUtc
            FROM _
            WHERE type = 'employee'
            ORDER BY lastName ASC, firstName ASC";

        using var query = _collection.CreateQuery(sql);
        var employees = new List<Employee>();

        foreach (var row in query.Execute())
        {
            employees.Add(new Employee
            {
                Id = row.GetString("id") ?? "",
                FirstName = row.GetString("firstName") ?? "",
                LastName = row.GetString("lastName") ?? "",
                Email = row.GetString("email") ?? "",
                Department = row.GetString("department") ?? "",
                Salary = row.GetDouble("salary"),
                CreatedAtUtc = row.GetString("createdAtUtc"),
                UpdatedAtUtc = row.GetString("updatedAtUtc")
            });
        }

        return employees;
    }

    public bool Create(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        if (_collection.GetDocument(employee.Id) is not null) return false;

        using var doc = new MutableDocument(employee.Id);
        WriteFields(doc, employee);
        doc.SetString("type", "employee");
        doc.SetString("createdAtUtc",
            employee.CreatedAtUtc ?? DateTimeOffset.UtcNow.ToString("O"));
        _collection.Save(doc);
        return true;
    }

    public bool Update(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        var existing = _collection.GetDocument(employee.Id);
        if (existing is null) return false;

        using var mutable = existing.ToMutable();
        WriteFields(mutable, employee);
        mutable.SetString("type", "employee");
        mutable.SetString("createdAtUtc",
            existing.GetString("createdAtUtc") ?? DateTimeOffset.UtcNow.ToString("O"));
        mutable.SetString("updatedAtUtc", DateTimeOffset.UtcNow.ToString("O"));
        _collection.Save(mutable);
        return true;
    }

    public bool Delete(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var existing = _collection.GetDocument(id);
        if (existing is null) return false;

        _collection.Delete(existing);
        return true;
    }

    private static Employee Map(Document doc) => new()
    {
        Id = doc.Id,
        FirstName = doc.GetString("firstName") ?? "",
        LastName = doc.GetString("lastName") ?? "",
        Email = doc.GetString("email") ?? "",
        Department = doc.GetString("department") ?? "",
        Salary = doc.GetDouble("salary"),
        CreatedAtUtc = doc.GetString("createdAtUtc"),
        UpdatedAtUtc = doc.GetString("updatedAtUtc")
    };

    private static void WriteFields(MutableDocument doc, Employee employee)
    {
        doc.SetString("firstName", employee.FirstName);
        doc.SetString("lastName", employee.LastName);
        doc.SetString("email", employee.Email);
        doc.SetString("department", employee.Department);
        doc.SetDouble("salary", employee.Salary);
    }
    public string? GetRawJsonById(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        var document = _collection.GetDocument(id);
        return document?.ToJSON();
    }

}
