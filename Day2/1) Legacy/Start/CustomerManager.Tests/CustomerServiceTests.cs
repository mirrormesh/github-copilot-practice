using CustomerManager.Repositories;
using CustomerManager.Services;
using Xunit;

namespace CustomerManager.Tests;

public class CustomerServiceTests
{
    [Fact]
    public void GetAllCustomers_ReturnsSeedCustomers()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());

        var customers = service.GetAllCustomers();

        Assert.NotNull(customers);
        Assert.True(customers.Count >= 3);
    }

    [Fact]
    public void AddCustomer_AddsCustomerWithNewId()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());

        var added = service.AddCustomer("Alice", "alice@example.com");

        Assert.True(added.Id > 0);
        Assert.Equal("Alice", added.Name);
        Assert.Equal("alice@example.com", added.Email);
        Assert.NotNull(service.GetCustomer(added.Id));
    }

    [Fact]
    public void UpdateCustomer_ExistingCustomer_UpdatesFields()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());
        var added = service.AddCustomer("Before", "before@example.com");

        var updated = service.UpdateCustomer(added.Id, "After", "after@example.com");

        Assert.NotNull(updated);
        Assert.Equal("After", updated!.Name);
        Assert.Equal("after@example.com", updated.Email);
    }

    [Fact]
    public void DeleteCustomer_ExistingCustomer_ReturnsTrueAndRemoves()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());
        var added = service.AddCustomer("DeleteMe", "delete@example.com");

        var deleted = service.DeleteCustomer(added.Id);

        Assert.True(deleted);
        Assert.Null(service.GetCustomer(added.Id));
    }

    [Fact]
    public void SearchCustomer_FindsByPartialName_CaseInsensitive()
    {
        var service = new CustomerService(new InMemoryCustomerRepository());
        service.AddCustomer("Charlie Brown", "charlie@example.com");

        var customer = service.SearchCustomer("charlie");

        Assert.NotNull(customer);
        Assert.Equal("Charlie Brown", customer!.Name);
    }
}
