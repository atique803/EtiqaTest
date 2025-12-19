using PayrollSystem.Application.Interfaces;
using PayrollSystem.Application.Services;
using PayrollSystem.Domain.Interfaces;
using PayrollSystem.Infrastructure.Data;
using PayrollSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register database services
builder.Services.AddSingleton<IDbConnectionFactory>(sp => new DbConnectionFactory(connectionString));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISkillsetRepository, SkillsetRepository>();

// Register application services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Run database migrations
try
{
    Console.WriteLine("Running database migrations...");
    DatabaseMigration.RunMigrations(connectionString);
    Console.WriteLine("Database migrations completed successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Database migration failed: {ex.Message}");
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
