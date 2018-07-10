using Demo2.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Demo2.Data
{
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext (DbContextOptions<AppDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Photo> Photos { get; set; }
    }
}
