using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class Account
    {
        [Required]
        [Display(Name = "Customer ID")]
        public long CustomerID { get; set; }
        //[Required]
        [Display(Name = "Account ID")]
        public long AccountID { get; set; }
        //[Required]
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; }
        [Required]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; }
        [Required] //add validation for min balance
        public long Balance { get; set; }
        //[Required]
        public DateTime CreateDate { get; set; }
        //[Required]
        public DateTime LastUpdated { get; set; }
        //[Required]
        public int Duration { get; set; }
    }
}
