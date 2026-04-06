using Asp.Versioning;
using IAM.API.Configure;
using IAM.API.Middlewares;
using IAM.Domain;
using IAM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Middlewares
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString(IamConst.Database.ConnectionString);
builder.Services.AddDbContext<IamDbContext>(options => options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

builder.Services.AddSharedInfrastructure(builder.Configuration);

DependencyInjection.RegisterUserContext(builder);

DependencyInjection.RegisterRepositories(builder);

DependencyInjection.RegisterOrchestrators(builder);

DependencyInjection.RegisterServices(builder);

DependencyInjection.RegisterValidators(builder);

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

JWTAuthentication.Configure(builder);

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

//Apply migrations and seed database in development
//await MigrateDatabase(app);
await PopulateDatabase(app);

app.Run();

static async Task MigrateDatabase(WebApplication app)
{
   if (app.Environment.IsDevelopment())
   {
      using var scope = app.Services.CreateScope();
      var dbIam = scope.ServiceProvider.GetRequiredService<IamDbContext>();
      dbIam.Database.Migrate();
      var dbShared = scope.ServiceProvider.GetRequiredService<SharedDbContext>();
      dbShared.Database.Migrate();
   }
}

static async Task PopulateDatabase(WebApplication app)
{
   if (app.Environment.IsDevelopment())
   {
      using var scope = app.Services.CreateScope();
      var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
      await seeder.SeedAsync();
   }
}
