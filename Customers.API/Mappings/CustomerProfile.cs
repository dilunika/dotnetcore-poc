using System;
using AutoMapper;
using Customers.API.Commands;
using Customers.API.Domain;

namespace Customers.API.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CreateCustomerRequest, Customer>();
            CreateMap<UpdateCustomerRequest, Customer>();
            CreateMap<Customer, UpdateCustomerRequest>();
        }
    }
}
