using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CashierDepositViewModel
    {
        public long CustomerID { get; set; }

        public long AccountID { get; set; }

        public string AccountType { get; set; }
        public long BalanceBeforeDeposit { get; set; }
        public long LatestBalance { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessage = "Amount should be greater than $1")]
        public long DepositAmount { get; set; }

    }
}
