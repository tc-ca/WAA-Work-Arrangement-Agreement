using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class DomainContext: DbContext
    {
        public DomainContext(DbContextOptions<DomainContext> options)
            : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TIPS");
            modelBuilder.Entity<Agreement>().Property(e => e.ObligationInd).HasConversion<int>();
            modelBuilder.Entity<Agreement>().Property(e => e.ConditionInd).HasConversion<int>();
            modelBuilder.Entity<Agreement>().Property(e => e.ArchivedInd).HasConversion<int>();
            modelBuilder.Entity<Agreement>().Property(e => e.MutualAgreementInd).HasConversion<int>();
            modelBuilder.Entity<Manager>().HasNoKey().ToView(null); ;

            modelBuilder.Entity<UserUnmetOHSItem>()
            .HasKey(nameof(UserUnmetOHSItem.UnMetOHSItemId), nameof(UserUnmetOHSItem.AgreementId));
        }
        public DbSet<TcEmail> TcEmails { get; set; }//EmergencyContact
        public DbSet<EmployeeContact> EmployeeContact { get; set; }
        public DbSet<TcOrganization> TcOrganizations { get; set; }
        public DbSet<TcUser> TcUsers { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Agreement> Agreements { get; set; }
        public DbSet<UserManager> UserManagers { get; set; }
        public DbSet<TcRegion> Regions { get; set; }        
        public DbSet<WorkSite> WorkSites { get; set; }
        public DbSet<OHSCategory> OHSCategories { get; set; }
        public DbSet<OHSChecklist> OHSChecklists { get; set; }        
        public DbSet<UserUnmetOHSItem> UserUnmetOHSItems { get; set; }
        public DbSet<DenyReason> DenyReasons { get; set; }
        public DbSet<Manager> Managers { get; set; }        
        public DbSet<AltWorkSite> AltWorkSites { get; set; }        
        public DbSet<SuperUser> SuperUsers { get; set; }        
        public DbSet<HybridOption> HybridOptions { get; set; }
        // public DbSet<SupportDocument> SupportDocuments { get; set; }
        public DbSet<FTRRecommender> FTRRecommenders { get; set; }        
        public DbSet<TMXMember> TMXMembers { get; set; }
    }
}
