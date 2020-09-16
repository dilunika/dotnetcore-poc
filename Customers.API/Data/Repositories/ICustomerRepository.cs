using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Customers.API.Domain;

namespace Customers.API.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> SaveCustomer(Customer createCustomerRequest);

        Task<Customer> GetCustomer(Guid customerId);

        Task<IEnumerable<Customer>> GetCustomers();

        Task<Boolean> CustomerExists(Guid customerId);

        Task UpdateCustomer(Customer updateCustomerRequest);
    }
}
