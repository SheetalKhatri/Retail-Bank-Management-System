using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class TransferViewModel
    {
        public Account SourceAccount { get; set; }
        [Required]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "Invalid Account ID")]
        public long TargetAccountID { get; set; }
        public List<Account> Accounts { get; set; }
        [Required]
        [Range(1,Double.PositiveInfinity,ErrorMessage ="Amount to transfer must be greater than 0")]
        public long AmountToTransfer { get; set; }
        public long TargetCustomerID { get; set; }
        public bool Searched { get; set; }
    }
}
