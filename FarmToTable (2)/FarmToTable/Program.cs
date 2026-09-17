using FarmToTable.API.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Allow actions to handle validation manually instead of automatic 400s
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<FarmToTableContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:5175",
                "https://localhost:5175",
                "http://localhost:3000",
                "https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure database is created to align schema with current models (avoid migration mismatches)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FarmToTableContext>();
    try
    {
        var recreate = app.Configuration.GetValue<bool>("RecreateDbOnStart");
        if (app.Environment.IsDevelopment() && recreate)
        {
            // Drop and recreate for clean dev schema
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
        else
        {
            db.Database.EnsureCreated();
        }

        // Seed sample products, categories, and farmers if database is empty
        await SampleDataSeeder.SeedAsync(db);
    }
    catch (Exception)
    {
        // In development, detailed exceptions will be shown by DeveloperExceptionPage
        // Avoid crashing startup on schema creation issues
    }
}

// Use CORS
app.UseCors("AllowReact");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
