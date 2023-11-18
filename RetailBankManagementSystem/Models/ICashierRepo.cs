using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Models
{
    public interface ICashierRepo
    {
        List<Account> GetAccounts();
        List<Account> GetAccountsByCustomer(long customerID);
        Account GetAccountByAccountID(long accountID);
        void Deposit(Account account, Transaction transaction);
        void Withdraw(Account account, Transaction transaction);
        void Transfer(Account sourceAccount, Account targetAccount, Transaction source);
        List<Account> ViewAllAccounts();
        public PagingList<AccountStatement> ViewStatement(long accountID, int PageIndex, int PageSize, int EffectivePageSize, int count, DateTime FromDate, DateTime ToDate);
        public int CountTransactions(long accountID, DateTime from, DateTime to);
    }
}
