using Microsoft.EntityFrameworkCore;
using HaberPortal.Models;

namespace HaberPortal.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<News> News { get; set; }
    }
}
