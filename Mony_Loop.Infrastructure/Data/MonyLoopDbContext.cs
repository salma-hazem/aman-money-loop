using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Entities.Marketplace___Applications;

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
        public DbSet<CircleRequest> CircleRequests { get; set; }
        public DbSet<Circle> Circles { get; set; }
        public DbSet<MarketplaceListing> MarketplaceListings { get; set; }
        public DbSet<CircleSlot> CircleSlots { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<MembershipApplication> MembershipApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(MonyLoopDbContext).Assembly);
        }
    }
}
