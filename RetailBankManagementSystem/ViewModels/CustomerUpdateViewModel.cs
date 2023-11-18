using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CustomerUpdateViewModel
    {
        public Customer CustomerDetails { get; set; }
        [Required]
        public string Address { get; set; }
        public List<string> States { get; set; }
        public List<string> Cities { get; set; }
    }
}
