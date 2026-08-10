using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.Agreement___Payment;

namespace Mony_Loop.Infrastructure.Data
{
    public class MonyLoopDbContext : DbContext
    {
        public MonyLoopDbContext(
            DbContextOptions<MonyLoopDbContext> options)
            : base(options)
        {
        }

        public DbSet<MembershipAgreement> MembershipAgreements { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(MonyLoopDbContext).Assembly);
        }
    }
}