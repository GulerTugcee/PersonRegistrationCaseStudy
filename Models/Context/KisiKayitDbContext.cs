using KisiKayitMVCApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KisiKayitMVCApp.Models.Context
{
    public class KisiKayitDbContext : DbContext
    {
        public KisiKayitDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Kisi> Kisiler { get; set; }
    }
}
