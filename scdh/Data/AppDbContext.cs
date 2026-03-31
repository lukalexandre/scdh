using Microsoft.EntityFrameworkCore;
using SCDH.Models;

namespace SCDH.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<ContratoHabitacional> Contratos { get; set; }
    }
}