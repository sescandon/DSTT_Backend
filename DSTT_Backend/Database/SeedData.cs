using Microsoft.EntityFrameworkCore;

namespace DSTT_Backend.Database
{
    public static class DataSeeder
    {
        public static void SeedData(IServiceProvider serviceProvider)
        {
            using (var context = new DsttDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<DsttDbContext>>()))
            {
                if (context.Users.Any())
                {
                    return;
                }

                context.Users.AddRange(
                    new User { Username = "Alfonso" },
                    new User { Username = "Ivan" },
                    new User { Username = "Alicia" }
                );


                context.SaveChanges();
            }
        }
    }
}
