using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class Customer
    {
        [Required(ErrorMessage ="Field cannot be blank")]
        [RegularExpression("^[0-9]{9}",ErrorMessage ="Invalid SSN")]
        public long SSN { get; set; }
        //[Required]
        public long CustomerID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        [Range(15, 150, ErrorMessage = "Must be between 15 and 150.")]
        public int Age { get; set; }
    }
}
