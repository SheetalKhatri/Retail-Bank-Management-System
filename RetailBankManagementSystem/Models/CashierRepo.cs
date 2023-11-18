using RetailBankManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public class CashierRepo : ICashierRepo
    {

        private BankManagementContext _context;

        public CashierRepo(BankManagementContext context)
        {
            _context = context;
        }

        public List<Account> GetAccounts()
        {
            return _context.Account.ToList();
        }
        public List<Account> ViewAllAccounts()
        {
            return _context.Account.ToList();
        }

        public void Deposit(Account account, Transaction transaction)
        {
            // Update account after deposit
            _context.Account.Update(account);
            // Save deposit operation as a transaction
            _context.Transaction.Add(transaction);

            _context.SaveChanges();
        }

        public List<Account> GetAccountsByCustomer(long customerID)
        {
            return _context.Account.Where(acc => acc.CustomerID == customerID).ToList();
        }

        public void Transfer(Account sourceAccount, Account targetAccount, Transaction source)
        {

            _context.Account.Update(sourceAccount);
            _context.Account.Update(targetAccount);

            _context.Transaction.Add(source);

            _context.SaveChanges();
        }

        public void Withdraw(Account account, Transaction transaction)
        {
            // Update account after deposit
            _context.Account.Update(account);
            // Save deposit operation as a transaction
            _context.Transaction.Add(transaction);

            _context.SaveChanges();
        }

        public Account GetAccountByAccountID(long accountID)
        {
            return _context.Account.FirstOrDefault(account => account.AccountID == accountID);
        }


        public int CountTransactions(long accountID, DateTime from, DateTime to)
        {
            Account account = GetAccountByAccountID(accountID);
            return _context.Transaction.Where(t => (t.SourceAccount == accountID || t.DestinationAccount == accountID) && t.TransactionDate >= from && t.TransactionDate < to.AddDays(1)).Count();
        }

        public PagingList<AccountStatement> ViewStatement(long accountID, int PageIndex, int PageSize, int EffectivePageSize, int count, DateTime FromDate, DateTime ToDate)
        {
            var transactions = (from transaction in _context.Transaction
                                where (transaction.SourceAccount == accountID || transaction.DestinationAccount == accountID) 
                                && transaction.TransactionDate >= FromDate && transaction.TransactionDate < ToDate.AddDays(1)
                                orderby transaction.TransactionDate descending
                                select new AccountStatement
                                {
                                    AccountID = accountID,
                                    TransactionID = transaction.TransactionID,
                                    TransactionType = transaction.TransactionType,
                                    Amount = transaction.Amount,
                                    TransactionDate = transaction.TransactionDate
                                });
            return PagingList<AccountStatement>.CreateAsync(transactions, PageIndex, PageSize, EffectivePageSize, count);
        }
    }
}
