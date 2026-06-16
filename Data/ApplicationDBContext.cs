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

        public bool IsUp100(int websiteID) => throw new NotSupportedException();
       
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDbFunction(typeof(ApplicationDBContext).GetMethod(nameof(IsUp100), new[] { typeof(int) })!).HasName("IsUp100");
            
        }
    }
}
