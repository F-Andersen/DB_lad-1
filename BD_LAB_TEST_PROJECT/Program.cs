using Microsoft.EntityFrameworkCore;
using BD_LAB_TEST_PROJECT.Extensions;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCustomServices(configuration);

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Prometheus HTTP metrics (request count, duration, status codes)
app.UseHttpMetrics();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapControllers();

// Prometheus metrics endpoint
app.MapMetrics();

app.UseExceptionHandler(options => { });
app.Run();
