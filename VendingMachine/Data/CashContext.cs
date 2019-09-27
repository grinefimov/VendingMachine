using Microsoft.EntityFrameworkCore;
using VendingMachine.Models;

namespace VendingMachine.Data
{
    public class CashContext : DbContext
    {
        public CashContext(DbContextOptions<CashContext> options)
            : base(options)
        {
        }

        public DbSet<Cash> Cashes { get; set; }
    }
}
