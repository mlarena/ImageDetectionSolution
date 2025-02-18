using ImageDetectionApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageDetectionApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Detection> Detections { get; set; }
    }
}
