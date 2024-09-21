var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// Middleware para manejar rutas desconocidas
app.Use(async (context, next) =>
{
    await next.Invoke();

    // Si no se encontró la ruta, imprimimos un mensaje
    if (context.Response.StatusCode == 404)
    {
        Console.WriteLine($"Ruta desconocida: {context.Request.Path}");
    }
});

// Mapa para obtener todas las rutas configuradas
app.MapGet("/routes", () =>
{
    var routes = app.Services.GetService<EndpointDataSource>()
        ?.Endpoints
        .Select(endpoint => endpoint.DisplayName)
        .ToList();

    return routes ?? new List<string> { "No se encontraron rutas." };
}).WithName("GetRoutes");

// Ejemplo de ruta existente
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

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

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
