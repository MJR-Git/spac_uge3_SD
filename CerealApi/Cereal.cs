using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace CerealApi.Models
{
    /*Here I define the Cereal class, and the the database context*/
    public class Cereal
    {
          public int Id { get; set; }
          public required string Name { get; set; }
          public required string Mfr { get; set; }
          public required string Type { get; set; }
          public required int Calories { get; set; }
          public required int Protein { get; set; }
          public required int Fat { get; set; }
          public required int Sodium { get; set; }
          public required float Fiber { get; set; }
          public required float Carbo { get; set; }
          public required int Sugar { get; set; }
          public required int Potass { get; set; }
          public required int Vitamins { get; set; }
          public required int Shelf { get; set; }
          public required float Weight { get; set; }
          public required float Cups { get; set; }
          public required float Rating { get; set; }
          public required string Image { get; set; }
    }

    public class CerealDb : IdentityDbContext<IdentityUser>
    {
        public CerealDb(DbContextOptions options) : base(options) { }

        public DbSet<Cereal> Cereals { get; set; } = null!;

    }

}