using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Stock.Models;

namespace Stock.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<WalletViewModel> Wallets { get; set; }
        public DbSet<TransactionViewModel> Transactions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
    }


}
