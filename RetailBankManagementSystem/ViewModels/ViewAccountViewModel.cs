using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class ViewAccountViewModel
    {
        public PagingList<Account> Accounts { get; set; }
        [RegularExpression("([0-9]{9,})$", ErrorMessage = "Account ID should be atleast 9 digits")]
        public long? AccountID { get; set; }
        public int? PageIndex { get; set; }
        public ViewAccountViewModel()
        {
            Accounts = PagingList<Account>.CreateEmpty();
            PageIndex = 1;
        }
    }
}
