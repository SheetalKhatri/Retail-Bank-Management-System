using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class AccountStatement
    {
        public long AccountID { get; set; }
        public long TransactionID { get; set; }
        public string TransactionType {get; set;}
        public long Amount { get; set; }
        public DateTime TransactionDate {get; set;}
    }
}
