using System.Reflection;
using System.Text.Json;
using API.Middleware;
using API.Utils.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Streamling.Data;
using Streamling.Filter;
using Streamling.Repository;
using Streamling.Service;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<HostawayReservationService>();
builder.Services.AddScoped<ConnectTeamService>();
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<ReservationRepository>();
builder.Services.AddScoped<PropertyRepository>();
builder.Services.AddScoped<ChannelRepository>();
builder.Services.AddScoped<ReservationService>();


builder.Services.AddHostedService<ReservationUpdateService>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<DateUnspecifiedExceptionFilter>();
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
        options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "CleanBoss API 🔮",
        Description = "An ASP.NET Core Web API for CleanBoss streamlining application. "
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
}
);

// Configure MySQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StoreContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5150);
});

var app = builder.Build();


// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }
app.UseCors("AllowAllOrigins");
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

var scope = app.Services.CreateScope();

var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();
try
{
    //create database if not exists
    context.Database.Migrate();
    DBInitializer.Initialize(context);
    logger.LogInformation("Database seeded successfully.");
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while seeding the database.");
}

app.Run();
