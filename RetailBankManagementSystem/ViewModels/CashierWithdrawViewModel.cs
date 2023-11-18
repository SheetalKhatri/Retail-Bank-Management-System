using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CashierWithdrawViewModel
    {
        public long CustomerID { get; set; }
        public long AccountID { get; set; }
        public string AccountType { get; set; }
        public long BalanceBeforeWithdrawal { get; set; }
        public long LatestBalance { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Amount should be greater than $1")]
        [ValidateWithdrawalAmount("LatestBalance", ErrorMessage = "Insufficient funds to withdraw")]
        public long WithdrawalAmount { get; set; }
    }
}
