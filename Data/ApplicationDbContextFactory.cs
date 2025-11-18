using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace wsahRecieveDelivary.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
           "Server = UODY-MIS\\SQLEXPRESS;Database=wsahRecieveDelivary; User Id=udoy;Password=udoy; TrustServerCertificate=True;MultipleActiveResultSets=true"
       );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}