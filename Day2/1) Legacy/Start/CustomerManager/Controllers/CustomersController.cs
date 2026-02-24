using Microsoft.AspNetCore.Mvc;
using CustomerManager.Models;
using CustomerManager.Services;

namespace CustomerManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public ActionResult<List<Customer>> GetAllCustomers()
    {
        var customers = _customerService.GetAllCustomers();
        return Ok(customers);
    }

    [HttpGet("search")]
    public ActionResult<Customer?> SearchCustomer([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest("Customer name is required");
        }

        // TODO: Service 호출해서 고객 조회
        var customer = _customerService.SearchCustomer(name);
        if (customer == null)
        {
            return NotFound($"Customer '{name}' not found");
        }

        return Ok(customer);
    }

    [HttpGet("{id}")]
    public ActionResult<Customer?> GetCustomer(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid customer ID");
        }

        var customer = _customerService.GetCustomer(id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    public ActionResult<Customer> AddCustomer([FromBody] CreateCustomerRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Name and Email are required");
        }

        var customer = _customerService.AddCustomer(request.Name, request.Email);
        return CreatedAtAction(nameof(GetCustomer), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public ActionResult<Customer> UpdateCustomer(int id, [FromBody] UpdateCustomerRequest request)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid customer ID");
        }

        if (request == null || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest("Name and Email are required");
        }

        var updatedCustomer = _customerService.UpdateCustomer(id, request.Name, request.Email);
        if (updatedCustomer == null)
        {
            return NotFound();
        }

        return Ok(updatedCustomer);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid customer ID");
        }

        var isDeleted = _customerService.DeleteCustomer(id);
        if (!isDeleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
