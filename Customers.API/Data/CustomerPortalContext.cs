using Customers.API.Domain;
using Microsoft.EntityFrameworkCore;


namespace Customers.API.Data
{
    public class CustomerPortalContext: DbContext
    {
        public CustomerPortalContext(DbContextOptions<CustomerPortalContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<Customer> Customers { get; set; }
    }
}
