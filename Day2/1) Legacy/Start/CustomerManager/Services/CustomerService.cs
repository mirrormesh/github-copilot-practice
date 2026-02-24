using CustomerManager.Models;

namespace CustomerManager.Services;

public interface ICustomerService
{
    Customer? GetCustomer(int id);
    Customer? SearchCustomer(string name);
    List<Customer> GetAllCustomers();
    Customer AddCustomer(string name, string email);
    Customer? UpdateCustomer(int id, string name, string email);
    bool DeleteCustomer(int id);
}

public class CustomerService : ICustomerService
{
    private readonly List<Customer> _customers =
    [
        new Customer { Id = 1, Name = "John Doe", Email = "john@example.com", CreatedAt = DateTime.Now },
        new Customer { Id = 2, Name = "Jane Smith", Email = "jane@example.com", CreatedAt = DateTime.Now },
        new Customer { Id = 3, Name = "Bob Wilson", Email = "bob@example.com", CreatedAt = DateTime.Now }
    ];

    public Customer? GetCustomer(int id)
    {
        return _customers.FirstOrDefault(c => c.Id == id);
    }

    public Customer? SearchCustomer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return _customers.FirstOrDefault(c => 
            c.Name != null && c.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public List<Customer> GetAllCustomers()
    {
        return _customers;
    }

    public Customer AddCustomer(string name, string email)
    {
        var nextId = _customers.Count == 0 ? 1 : _customers.Max(c => c.Id) + 1;
        var customer = new Customer
        {
            Id = nextId,
            Name = name,
            Email = email,
            CreatedAt = DateTime.Now
        };

        _customers.Add(customer);
        return customer;
    }

    public Customer? UpdateCustomer(int id, string name, string email)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return null;
        }

        customer.Name = name;
        customer.Email = email;
        return customer;
    }

    public bool DeleteCustomer(int id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null)
        {
            return false;
        }

        return _customers.Remove(customer);
    }
}
