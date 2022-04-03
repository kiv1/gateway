using api_gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Authentication;
using Ocelot.Middleware;

IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("./Configurations/ocelot.json")
    .Build();
var builder = WebApplication.CreateBuilder(args);
//builder.Services.ConfigureDownstreamHostAndPortsPlaceholders(configuration);

// Add services to the container.
builder.Services.AddOcelot(configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var authenticationProviderKey = "TestKey";

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
.AddJwtBearer(authenticationProviderKey, x =>
{
    x.Authority = Environment.GetEnvironmentVariable("AUTHORITY_URL") ?? "https://dev-s4jkne9p.us.auth0.com/";
    x.Audience = Environment.GetEnvironmentVariable("AUDIENCE_NAME") ?? "gateway";
});
builder.Services.AddCors();
var app = builder.Build();
app.UseOcelot();
app.UseCors(b => b
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
);
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();