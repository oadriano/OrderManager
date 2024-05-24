using OrderManager.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderManager.ConstValues;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManager.Data
{
    public class DBInitialization : DbContext
    {
        public DbSet<OrderModel> Auftraege { get; set; }
        public DbSet<ProcessModel> Vorgaenge { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder
                .UseSqlServer(ConstStrings.CONNECTIONSTRING)
                .LogTo(Console.WriteLine, LogLevel.Information);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderModel>(entity =>
            {
                entity.HasMany(a => a.VorgaengeInAuftrag)
               .WithOne(v => v.AuftragInVorgang)
               .HasForeignKey(v => v.AuftragID);

                entity.ToTable(tb => tb.HasTrigger("Auftraege_Insert"));
            });

            modelBuilder.Entity<ProcessModel>()
            .ToTable(tb => tb.HasTrigger("x"));
        }
    }
}
