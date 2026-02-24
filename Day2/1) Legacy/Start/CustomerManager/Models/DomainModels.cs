namespace CustomerManager.Models;


public class Customer
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime OrderDate { get; set; }
}

public class HealthResponse
{
    public string? Status { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; }
}

public class CreateCustomerRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class UpdateCustomerRequest
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}
