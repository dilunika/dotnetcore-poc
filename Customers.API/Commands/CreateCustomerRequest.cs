using System;
using System.ComponentModel.DataAnnotations;

namespace Customers.API.Commands
{
    public class CreateCustomerRequest : ManipulateCustomerRequest
    {

        [Required(ErrorMessage = "Email is mandatory to create customer")]
        [EmailAddress]
        public string Email { get; set; }

    }
}
