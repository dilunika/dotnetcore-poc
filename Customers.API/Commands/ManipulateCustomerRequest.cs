using System;
using System.ComponentModel.DataAnnotations;

namespace Customers.API.Commands
{
    public abstract class ManipulateCustomerRequest
    {
        [Required(ErrorMessage = "First name is mandatory to create customer")]
        public string FirstNames { get; set; }

        [Required(ErrorMessage = "Last name is mandatory to create customer")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Database is mandatory to create customer")]
        public DateTime DateOfBirth { get; set; }
    }
}
