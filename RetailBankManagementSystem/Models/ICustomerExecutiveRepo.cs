using RetailBankManagementSystem.Controllers;
using RetailBankManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public interface ICustomerExecutiveRepo
    {
        void AddCustomer(Customer customer);
        void DeleteCustomer(Customer customer);
        void EditCustomer(Customer customer);
        List<Customer> ViewAllCustomers();
        void AddAccount(Account account);
        void DeleteAccount(Account account);
        //Account GetAccountByCustomerID(long CustomerID);
        PagingList<Account> ViewAccounts(int PageIndex, long ?AccountID);
        Customer getCustomerByCID(long id);
        Customer getCustomerBySsn(long id);
        CustomerAccViewModel viewStatus();
        Account GetAccountByAccountID(long AccountID);
        public List<Account> GetAllAccountsByCustomerID(long CustomerID);
        void ActivateAccount(Account account);

        public Account GetAccountByCustomerID(long cid);
        //public List<CustomerAcc> GetPaginatedResult(int currentPage, int pageSize = 5);
        //public int getCount();
    }
}
