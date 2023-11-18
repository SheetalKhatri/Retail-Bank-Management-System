using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class ValidateWithdrawalAmount: ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public ValidateWithdrawalAmount(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (long)value;
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if(property == null)
            {
                throw new ArgumentException("Property with this name was not found");
            }
            var comparisonValue = (long)property.GetValue(validationContext.ObjectInstance);
            if(currentValue > comparisonValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
