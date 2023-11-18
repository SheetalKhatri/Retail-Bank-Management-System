using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CustomerAcc
    {
        public long SSN { get; set; }
        public long CustomerID { get; set; }
        public string Name { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Status { get; set; }
        public int Age { get; set; }
    }
}
