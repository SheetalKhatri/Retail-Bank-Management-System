using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CashierSearchViewModel
    {
        [RegularExpression("([0-9]{9,})$", ErrorMessage = "Customer ID should be atleast 9 digits")]
        public long CustomerID { get; set; }
        [RegularExpression("([0-9]{9,})$", ErrorMessage = "Account ID should be atleast 9 digits")]
        public long AccountID { get; set; }
        public List<Account> Accounts { get; set; }
    }
}
