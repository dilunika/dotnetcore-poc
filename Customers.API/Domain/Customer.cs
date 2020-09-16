using System;
using System.ComponentModel.DataAnnotations;

namespace Customers.API.Domain
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string FirstNames { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
