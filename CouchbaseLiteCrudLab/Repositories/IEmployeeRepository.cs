using CouchbaseLiteCrudLab.Models;

namespace CouchbaseLiteCrudLab.Repositories;

public interface IEmployeeRepository
{
    Employee? GetById(string id);
    IReadOnlyList<Employee> GetAll();
    bool Create(Employee employee);
    bool Update(Employee employee);
    bool Delete(string id);
    string? GetRawJsonById(string id);
}
