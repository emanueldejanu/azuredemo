using Demo1.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Demo1.Data
{
    public class AppDatabaseContext : DbContext
    {
        public AppDatabaseContext (DbContextOptions<AppDatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<TodoEntity> Todo { get; set; }
    }
}
