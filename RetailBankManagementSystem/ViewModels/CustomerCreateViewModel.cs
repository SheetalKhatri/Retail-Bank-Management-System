using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CustomerCreateViewModel
    {
        public Customer CustomerDetails { get; set; }
        [Required(ErrorMessage ="The Address field is required.")]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        public List<string> States { get; set; }
        public List<string> Cities { get; set; }
    }
}
