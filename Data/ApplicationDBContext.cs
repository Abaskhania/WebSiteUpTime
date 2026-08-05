using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using SatraWebApplication.Model;


namespace SatraWebApplication.Data
{
    public class ApplicationDBContext :DbContext
    {
        public DbSet<WebSiteUpTime> WebSiteUpTime { get; set; }
        public DbSet<WebSite> WebSite { get; set; }
        public DbSet<SatraUser> SatraUser { get; set; }
        public DbSet<Report1Result> Report1Results { get; set; }
        public DbSet<UserLoginLog> UserLoginLogs { get; set; }
        public bool IsUp100(int websiteID) => throw new NotSupportedException();
       
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(ApplicationDBContext).GetMethod(nameof(IsUp100), new[] { typeof(int) })!).HasName("IsUp100");
            modelBuilder.Entity<Report1Result>()
            .HasNoKey()
            .ToView(null);

        }
    }
}
