using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Customers.API.Commands;
using Customers.API.Data.Repositories;
using Customers.API.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customers.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        private readonly ILogger<CustomerController> _logger;

        private readonly IMapper _mapper;

        public CustomerController(ICustomerRepository customerRepository, ILogger<CustomerController> logger, IMapper mapper)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(CreateCustomerRequest createCustomerRequest)
        {

            Customer customer = _mapper.Map<Customer>(createCustomerRequest);
            await _customerRepository.SaveCustomer(customer);

            _logger.LogDebug("Customer created successfully.");

            var resourceUrl = $"api/customers/{customer.Id}";
            return Created(resourceUrl, customer);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            var customers = await _customerRepository.GetCustomers();

            return Ok(customers);
        }

        [HttpGet("{customerId}")]
        public async Task<ActionResult<Customer>> GetCustomer(Guid customerId)
        {
            var customer = await _customerRepository.GetCustomer(customerId);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [HttpPatch("{customerId}")]
        public async Task<ActionResult<Customer>> UpdateCustomer(Guid customerId, JsonPatchDocument<UpdateCustomerRequest> patchDocument)
        {
            var customer = await _customerRepository.GetCustomer(customerId);

            if (customer == null)
            {
                return NotFound();
            }

            UpdateCustomerRequest updateCustomerRequest = _mapper.Map<UpdateCustomerRequest>(customer);

            patchDocument.ApplyTo(updateCustomerRequest, ModelState);

            _mapper.Map(updateCustomerRequest, customer);

            await _customerRepository.UpdateCustomer(customer);

            return Ok(customer);
        }
    }
}
