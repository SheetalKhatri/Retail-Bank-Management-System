using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.ViewModels
{
    public class CustSearch
    {
        [Required(ErrorMessage = "Field cannot be blank")]
        [RegularExpression("^[0-9]{9}$", ErrorMessage = "SSN Should be 9 Digits")]
        public long SSN { get; set; }
        //[Required]
        [RegularExpression("^[0-9]{9,10}$", ErrorMessage = "Customer ID Should be atleast 9 Digits")]
        public long CustomerID { get; set; }
    }
}
