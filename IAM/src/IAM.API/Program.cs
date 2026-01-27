using IAM.Core.Services;
using IAM.Domain.Repositories;
using IAM.Domain.QueryRepositories;
using IAM.Infrastructure;
using IAM.Infrastructure.QueryRepositories;
using IAM.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IAM.API.Configure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add DbContext
builder.Services.AddDbContext<IamDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IAM")));

DependencyInjection.RegisterRepositories(builder);

DependencyInjection.RegisterServices(builder);

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-jwt-key-here-make-it-long-and-secure";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
       options.TokenValidationParameters = new TokenValidationParameters
       {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = "IAM.API",
          ValidAudience = "IAM.Client",
          IssuerSigningKey = new SymmetricSecurityKey(key)
       };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//Seed database in development
if (app.Environment.IsDevelopment())
{
   using var scope = app.Services.CreateScope();
   var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
   await seeder.SeedAsync();
}

app.Run();