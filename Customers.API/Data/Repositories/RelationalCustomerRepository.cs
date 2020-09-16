using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Customers.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace Customers.API.Data.Repositories
{
    public class RelationalCustomerRepository : ICustomerRepository
    {
        private readonly CustomerPortalContext _context;

        public RelationalCustomerRepository(CustomerPortalContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> CustomerExists(Guid customerId)
        {
            if (customerId == Guid.Empty) throw new ArgumentNullException(nameof(customerId));

            return await _context.Customers.AnyAsync(c => c.Id == customerId);
        }

        public async Task<Customer> GetCustomer(Guid customerId)
        {
            if (customerId == Guid.Empty) throw new ArgumentNullException(nameof(customerId));

            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> SaveCustomer(Customer customer)
        {
            if (customer == null) throw new ArgumentNullException(nameof(customer));

            customer.Id = Guid.NewGuid();
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task UpdateCustomer(Customer updateCustomerRequest)
        {
            _context.Customers.Update(updateCustomerRequest);
            await _context.SaveChangesAsync();
        }
    }
}
