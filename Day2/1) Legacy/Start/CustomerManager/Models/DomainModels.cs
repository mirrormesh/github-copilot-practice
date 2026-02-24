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

public class AgentChatRequest
{
    public string? Prompt { get; set; }
}

public class AgentChatResponse
{
    public string? Response { get; set; }
    public string? Model { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
}
