using cash_register.Data;
using Microsoft.EntityFrameworkCore;

namespace cash_register
{
    public class CashRegisterDataContext : DbContext
    {
        public CashRegisterDataContext(DbContextOptions<CashRegisterDataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<ReceiptLine> ReceiptLines { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
    }
}
