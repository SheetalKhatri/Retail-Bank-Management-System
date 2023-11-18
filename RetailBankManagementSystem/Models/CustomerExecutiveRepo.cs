using RetailBankManagementSystem.Controllers;
using RetailBankManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class CustomerExecutiveRepo : ICustomerExecutiveRepo
    {
        public BankManagementContext _context;

        public CustomerExecutiveRepo(BankManagementContext context)
        {
            _context = context;
        }


        public void AddAccount(Account account)
        {
            _context.Account.Add(account);
            _context.SaveChanges();
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customer.Add(customer);
            _context.SaveChanges();
        }

        public void DeleteAccount(Account account)
        {
            _context.Account.Remove(account);
            _context.SaveChanges();
        }

        public void DeleteCustomer(Customer customer)
        {
            List<Transaction> transactions = _context.Transaction.Where(trans => trans.CustomerID == customer.CustomerID).ToList();
            foreach (Transaction transaction in transactions)
            {
                _context.Transaction.Remove(transaction);
                _context.SaveChanges();
            }
            _context.Customer.Remove(customer);
            
            _context.SaveChanges();
        }

        public void EditCustomer(Customer customer)
        {
            _context.Customer.Update(customer);
            _context.SaveChanges();
        }

        public Customer getCustomerByCID(long id)
        {

            return _context.Customer.FirstOrDefault(customer => customer.CustomerID == id);
        }

        public Customer getCustomerBySsn(long ssn)
        {

            return _context.Customer.FirstOrDefault(customer => customer.SSN == ssn);
        }

        public PagingList<Account> ViewAccounts(int PageIndex, long ?AccountID)
        {
            int PageSize = 5;
            var query = _context.Account.Where(account => AccountID == null || account.AccountID == AccountID);          
            return PagingList<Account>.CreateAsync(query, PageIndex, PageSize);
        }

        public List<Customer> ViewAllCustomers()
        {
            //int PageSize = 3;
            //var query = _context.Customer.ToList().AsQueryable<Customer>();
            //return PagingList<Customer>.CreateAsync(query, PageIndex, PageSize);
            return _context.Customer.ToList();
        }

        public CustomerAccViewModel viewStatus()
        {
            var customerDetails = (from customer in _context.Customer
                                   join account in _context.Account on customer.CustomerID equals account.CustomerID
                                   select new CustomerAcc
                                   {
                                       CustomerID = customer.CustomerID,
                                       Name = customer.Name,
                                       SSN = customer.SSN,
                                       Address = customer.Address,
                                       Age = customer.Age,
                                       City = customer.City,
                                       State = customer.State,
                                       Status = account.AccountStatus
                                   }).ToList();
            CustomerAccViewModel customerAccViewModel = new CustomerAccViewModel();
            customerAccViewModel.customerAccs = customerDetails;
            return customerAccViewModel;
        }

        

        public Account GetAccountByAccountID(long accountID)
        {
            return _context.Account.FirstOrDefault(a => a.AccountID == accountID);
        }

        public Account GetAccountByCustomerID(long CustomerID)
        {
            return _context.Account.FirstOrDefault(a => a.CustomerID == CustomerID);
        }

        public List<Account> GetAllAccountsByCustomerID(long CustomerID)
        {
            return _context.Account.Where(account => account.CustomerID == CustomerID).ToList();
        }

    
        public void ActivateAccount(Account account)
        {
            account.AccountStatus = "active";
            _context.Account.Update(account);
            _context.SaveChanges();
            
        }
    }
}
