using Asp.Versioning;
using IAM.API.Configure;
using IAM.API.Middlewares;
using IAM.Application.Services;
using IAM.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Middlewares
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add DbContext
builder.Services.AddDbContext<IamDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IAM")).
    UseSnakeCaseNamingConvention());

DependencyInjection.RegisterRepositories(builder);

DependencyInjection.RegisterOrchestrators(builder);

DependencyInjection.RegisterServices(builder);

DependencyInjection.RegisterValidators(builder);

JWTAuthentication.Configure(builder);

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
   options.DefaultApiVersion = new ApiVersion(1);
   options.ReportApiVersions = true;
   options.AssumeDefaultVersionWhenUnspecified = true;
   options.ApiVersionReader = ApiVersionReader.Combine(
       new UrlSegmentApiVersionReader(),
       new HeaderApiVersionReader("X-Api-Version"));
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();

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

//Apply migrations and seed database in development
//await PopulateDatabase(app);

app.Run();

static async Task PopulateDatabase(WebApplication app)
{
   if (app.Environment.IsDevelopment())
   {
      using var scope = app.Services.CreateScope();
      var db = scope.ServiceProvider.GetRequiredService<IamDbContext>();
      db.Database.Migrate();

      var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
      await seeder.SeedAsync();
   }
}