using BudgetMgrServiceLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetMgrService_WebAPI.Models
{

    public class TransactionContext : DbContext
    {


        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options)
        {
        }

        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<FilePath> ContentFilePath { get; set; }

        public DbSet<CreditTypeRef> CreditTypesOfInterest { set; get; }
        public DbSet<DebitTypeRef> DebitTypesOfInterest { set; get; }
        public DbSet<TransName> TransNames { set; get; }
        public DbSet<TransType> TransTypes { set; get; }

        public DbSet<TransGroupType> TransGroupTypes { set; get; }

        public DbSet<TransactionGroupEx> TransGroups { set; get; }

        public DbSet<TransactionNameEx> NameRefs { set; get; }
    }

}