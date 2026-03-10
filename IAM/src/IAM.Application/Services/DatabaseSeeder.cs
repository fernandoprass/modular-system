using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;

namespace IAM.Application.Services;

public interface IDatabaseSeeder
{
   Task SeedAsync();
}

public class DatabaseSeeder : IDatabaseSeeder
{
   private readonly IUnitOfWork _unitOfWork;
   private const string DefaultPassword = "Password123!";

   public DatabaseSeeder(IUnitOfWork unitOfWork)
   {
      _unitOfWork = unitOfWork;
   }

   public async Task SeedAsync()
   {
      await SeedBeatlesAsync();
      await SeedRollingStonesAsync();
      await SeedGunsAndRosesAsync();
      await SeedMetallicaAsync();

      await _unitOfWork.SaveChangesAsync();
   }

   private async Task SeedBeatlesAsync()
   {
      var bandId = Guid.Parse("b0000000-0000-0000-0000-000000000001");

      if (await _unitOfWork.Customers.ExistsAsync(bandId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = bandId,
         Name = "The Beatles",
         Code = "BEATLES",
         Type = CustomerType.Company,
         Description = "The Fab Four from Liverpool",
         CreatedAt = DateTime.UtcNow
      });

      var members = new[]
      {
            ("John Lennon", "john@beatles.com"),
            ("Paul McCartney", "paul@beatles.com"),
            ("George Harrison", "george@beatles.com"),
            ("Ringo Starr", "ringo@beatles.com")
        };

      foreach (var (name, email) in members)
      {
         await _unitOfWork.Users.AddAsync(User.Create(name, email, Argon2.Hash(DefaultPassword), bandId));
      }
   }

   private async Task SeedRollingStonesAsync()
   {
      var bandId = Guid.Parse("b0000000-0000-0000-0000-000000000002");

      if (await _unitOfWork.Customers.ExistsAsync(bandId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = bandId,
         Name = "The Rolling Stones",
         Code = "STONES",
         Type = CustomerType.Company,
         Description = "The World's Greatest Rock and Roll Band",
         CreatedAt = DateTime.UtcNow
      });

      var members = new[]
      {
            ("Mick Jagger", "mick@stones.com"),
            ("Keith Richards", "keith@stones.com"),
            ("Ronnie Wood", "ronnie@stones.com"),
            ("Charlie Watts", "charlie@stones.com")
        };

      foreach (var (name, email) in members)
      {
         await _unitOfWork.Users.AddAsync(User.Create(name, email, Argon2.Hash(DefaultPassword), bandId));
      }
   }

   private async Task SeedGunsAndRosesAsync()
   {
      var bandId = Guid.Parse("b0000000-0000-0000-0000-000000000003");

      if (await _unitOfWork.Customers.ExistsAsync(bandId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = bandId,
         Name = "Guns N' Roses",
         Code = "GNR",
         Type = CustomerType.Company,
         Description = "The Most Dangerous Band in the World",
         CreatedAt = DateTime.UtcNow
      });

      var members = new[]
      {
            ("Axl Rose", "axl@gnr.com"),
            ("Slash", "slash@gnr.com"),
            ("Duff McKagan", "duff@gnr.com"),
            ("Izzy Stradlin", "izzy@gnr.com")
        };

      foreach (var (name, email) in members)
      {
         await _unitOfWork.Users.AddAsync(User.Create(name, email, Argon2.Hash(DefaultPassword), bandId));
      }
   }

   private async Task SeedMetallicaAsync()
   {
      var bandId = Guid.Parse("b0000000-0000-0000-0000-000000000004");

      if (await _unitOfWork.Customers.ExistsAsync(bandId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = bandId,
         Name = "Metallica",
         Code = "METALLICA",
         Type = CustomerType.Company,
         Description = "Thrash Metal Legends",
         CreatedAt = DateTime.UtcNow
      });

      var members = new[]
      {
            ("James Hetfield", "james@metallica.com"),
            ("Lars Ulrich", "lars@metallica.com"),
            ("Kirk Hammett", "kirk@metallica.com"),
            ("Robert Trujillo", "robert@metallica.com")
        };

      foreach (var (name, email) in members)
      {
         await _unitOfWork.Users.AddAsync(User.Create(name, email, Argon2.Hash(DefaultPassword), bandId));
      }
   }
}