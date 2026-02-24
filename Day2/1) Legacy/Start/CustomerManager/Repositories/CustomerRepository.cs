using CustomerManager.Models;

namespace CustomerManager.Repositories;

public interface ICustomerRepository
{
    Customer? GetById(int id);
    Customer? SearchByName(string name);
    List<Customer> GetAll();
    int GetNextId();
    Customer Add(Customer customer);
    Customer? Update(Customer customer);
    bool Delete(int id);
}

public class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers =
    [
        new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.Now },
        new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.Now },
        new Customer { Id = 3, Name = "Bob Wilson", Email = "bob@example.com", CreatedAt = DateTime.Now }
    ];

    public Customer? GetById(int id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public Customer? SearchByName(string name)
    {
        return _customers.FirstOrDefault(c =>
            c.Name != null && c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public List<Customer> GetAll()
    {
        return _customers;
    }

    public int GetNextId()
    {
        return _customers.Count == 0 ? 1 : _customers.Max(c => c.Id) + 1;
    }

    public Customer Add(Customer customer)
    {
        _customers.Add(customer);
        return customer;
    }

    public Customer? Update(Customer customer)
    {
        var existingCustomer = _customers.FirstOrDefault(c => c.Id == customer.Id);
        if (existingCustomer == null)
        {
            return null;
        }

        existingCustomer.Name = customer.Name;
        existingCustomer.Email = customer.Email;
        return existingCustomer;
    }

    public bool Delete(int id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return false;
        }

        return _customers.Remove(customer);
    }
}
