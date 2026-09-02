using Microsoft.EntityFrameworkCore;
using Historial.Api.Models;

namespace Historial.Api.Data
{
    public class HistorialDBContext : DbContext
    {
        public HistorialDBContext(DbContextOptions<HistorialDBContext> options) : base(options)
        {
        }

        public DbSet<HistorialClinico> HistorialClinico { get; set; } = default!;
    }
}