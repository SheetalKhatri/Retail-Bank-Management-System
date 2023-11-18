using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class Transaction
    {
        //[Required]
        public long TransactionID { get; set; }
        //[Required]
        public long CustomerID { get; set; }
        //[Required]
        public string AccountType { get; set; }
        //[Required]
        public string TransactionType { get; set; }
        //[Required]
        public long Amount { get; set; }
        //[Required]
        public DateTime TransactionDate { get; set; }
        public long SourceAccount { get; set; }
        public long DestinationAccount { get; set; }
    }
}
