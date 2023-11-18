using RetailBankManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CustomerAccDeleteModel
    {
        public Customer customer { get; set; }
        public List<Account> accounts { get; set; }

    }
}
