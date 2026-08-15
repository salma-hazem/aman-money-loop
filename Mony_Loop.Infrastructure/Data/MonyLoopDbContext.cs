using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Mony_Loop.Domain.Entities.Agreement___Payment;
using Mony_Loop.Domain.Entities.CircleRequestManagement;
using Mony_Loop.Domain.Entities.Marketplace___Applications;
using Mony_Loop.Domain.Entities.Onboarding___Member_Ledger;
using MonyLoop.Domain.Entities.UserAuth;
using System.Reflection;
using Document = Mony_Loop.Domain.Entities.Onboarding___Member_Ledger.Document;

namespace Mony_Loop.Infrastructure.Data
{
    public class MonyLoopDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public MonyLoopDbContext(DbContextOptions<MonyLoopDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
            modelBuilder.Entity<ApplicationRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<MembershipAgreement> MembershipAgreements { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<CircleRequest> CircleRequests { get; set; }
        public DbSet<Circle> Circles { get; set; }
        public DbSet<MarketplaceListing> MarketplaceListings { get; set; }
        public DbSet<CircleSlot> CircleSlots { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<MembershipApplication> MembershipApplications { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentRequirement> DocumentRequirements { get; set; }
        public DbSet<MemberLedger> MemberLedgers { get; set; }
        public DbSet<OnboardingCase> OnboardingCases { get; set; }
        public DbSet<OTPToken> OTPTokens { get; set; }
    }
}