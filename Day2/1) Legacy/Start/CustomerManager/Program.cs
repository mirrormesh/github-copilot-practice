using CustomerManager.Models;
using CustomerManager.Repositories;
using CustomerManager.Services;

var builder = WebApplication.CreateBuilder(args);

EnsureGitHubModelsConfiguration(builder.Configuration);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Legacy API",
        Version = "1.0.0",
        Description = "레거시 .NET API - Step 1: 기본 기동 테스트"
    });
});

builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<ICustomerService, CustomerService>();
builder.Services.AddSingleton<IGitHubModelsAgentService, GitHubModelsAgentService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () =>
    Results.Ok(new HealthResponse
    {
        Status = "Healthy",
        Message = "Legacy API is running",
        Timestamp = DateTime.UtcNow
    }));

var customers = app.MapGroup("/api/customers");

customers.MapGet("", (int? pageNumber, int? pageSize, ICustomerService customerService) =>
{
    var effectivePageNumber = pageNumber.GetValueOrDefault(1);
    var effectivePageSize = pageSize.GetValueOrDefault(10);

    if (effectivePageNumber <= 0)
    {
        effectivePageNumber = 1;
    }

    if (effectivePageSize <= 0)
    {
        effectivePageSize = 10;
    }

    var allCustomers = customerService.GetAllCustomers();
    var totalItems = allCustomers.Count;
    var totalPages = totalItems == 0
        ? 0
        : (int)Math.Ceiling(totalItems / (double)effectivePageSize);

    var items = allCustomers
        .Skip((effectivePageNumber - 1) * effectivePageSize)
        .Take(effectivePageSize)
        .ToList();

    var response = new PagedResponse<Customer>
    {
        Items = items,
        CurrentPage = effectivePageNumber,
        TotalPages = totalPages,
        TotalItems = totalItems
    };

    return Results.Ok(response);
});

customers.MapGet("/search", (string name, ICustomerService customerService) =>
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return Results.BadRequest("Customer name is required");
    }

    var customer = customerService.SearchCustomer(name);
    if (customer == null)
    {
        return Results.NotFound($"Customer '{name}' not found");
    }

    return Results.Ok(customer);
});

customers.MapGet("/{id:int}", (int id, ICustomerService customerService) =>
{
    var customer = customerService.GetCustomer(id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

customers.MapPost("", (CreateCustomerRequest request, ICustomerService customerService) =>
{
    if (!HasRequiredFields(request.Name, request.Email))
    {
        return Results.BadRequest("Name and Email are required");
    }

    var name = request.Name!;
    var email = request.Email!;
    var customer = customerService.AddCustomer(name, email);
    return Results.Created($"/api/customers/{customer.Id}", customer);
});

customers.MapPut("/{id:int}", (int id, UpdateCustomerRequest request, ICustomerService customerService) =>
{
    if (!HasRequiredFields(request.Name, request.Email))
    {
        return Results.BadRequest("Name and Email are required");
    }

    var name = request.Name!;
    var email = request.Email!;
    var updatedCustomer = customerService.UpdateCustomer(id, name, email);
    if (updatedCustomer == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(updatedCustomer);
});

customers.MapDelete("/{id:int}", (int id, ICustomerService customerService) =>
{
    var isDeleted = customerService.DeleteCustomer(id);
    return isDeleted ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/api/agent/chat", async (AgentChatRequest request, IGitHubModelsAgentService agentService) =>
{
    if (string.IsNullOrWhiteSpace(request.Prompt))
    {
        return Results.BadRequest("Prompt is required");
    }

    try
    {
        var chatResult = await agentService.ChatAsync(request.Prompt);
        return Results.Ok(new AgentChatResponse
        {
            Response = chatResult.Response,
            Model = chatResult.Model
        });
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
    }
});

static bool HasRequiredFields(string? name, string? email) =>
    !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(email);

static void EnsureGitHubModelsConfiguration(ConfigurationManager configuration)
{
    var token = configuration["GITHUB_MODELS_TOKEN"] ?? configuration["GITHUB_TOKEN"];
    if (!string.IsNullOrWhiteSpace(token))
    {
        return;
    }

    if (Console.IsInputRedirected)
    {
        return;
    }

    Console.Write("GITHUB_MODELS_TOKEN이 없습니다. 토큰을 입력하세요(없으면 Enter): ");
    var inputToken = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(inputToken))
    {
        return;
    }

    var endpoint = configuration["GITHUB_MODELS_ENDPOINT"] ?? "https://models.inference.ai.azure.com";
    var model = configuration["GITHUB_MODELS_MODEL"] ?? "gpt-4o-mini";

    configuration.AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["GITHUB_MODELS_TOKEN"] = inputToken,
        ["GITHUB_MODELS_ENDPOINT"] = endpoint,
        ["GITHUB_MODELS_MODEL"] = model
    });

    Console.WriteLine("입력한 토큰을 현재 실행 세션에 적용했습니다.");
}

app.Run();
