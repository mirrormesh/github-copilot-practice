using CustomerManager.Models;
using CustomerManager.Repositories;

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
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public Customer? GetCustomer(int id)
    {
        return _customerRepository.GetById(id);
    }

    public Customer? SearchCustomer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return _customerRepository.SearchByName(name);
    }

    public List<Customer> GetAllCustomers()
    {
        return _customerRepository.GetAll();
    }

    public Customer AddCustomer(string name, string email)
    {
        var nextId = _customerRepository.GetNextId();
        var customer = new Customer
        {
            Id = nextId,
            Name = name,
            Email = email,
            CreatedAt = DateTime.Now
        };

        return _customerRepository.Add(customer);
    }

    public Customer? UpdateCustomer(int id, string name, string email)
    {
        return _customerRepository.Update(new Customer
        {
            Id = id,
            Name = name,
            Email = email
        });
    }

    public bool DeleteCustomer(int id)
    {
        return _customerRepository.Delete(id);
    }
}
