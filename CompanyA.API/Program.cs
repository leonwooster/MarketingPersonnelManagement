using CompanyA.DataAccess;
using CompanyA.BusinessComponents;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<MarketingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register business services
builder.Services.AddScoped<IPersonnelService, PersonnelService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebUI", policy =>
    {
        policy.WithOrigins("http://localhost:5000", "https://localhost:5001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowWebUI");

app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
