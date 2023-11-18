using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailBankManagementSystem.Models;
using RetailBankManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Controllers
{
    [Authorize(Roles = "Cashier")]
    public class CashierController : Controller
    {

        private ICashierRepo _repo;

        public CashierController(ICashierRepo repo)
        {
            _repo = repo;
        }

        // Index Module
        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public IActionResult Index(int? pageNumber)
        {
            var accountsList = _repo.GetAccounts();
            int pageSize = 5;
            return View(PagingList<Account>.CreateAsync(
                accountsList.AsQueryable<Account>(), 
                pageNumber ?? 1, 
                pageSize));
        }

        // Search Module
        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public IActionResult Search()
        {
            CashierSearchViewModel cashierSearchViewModel = new CashierSearchViewModel
            {
                Accounts = null
            };
            return View(cashierSearchViewModel);
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public IActionResult Search(CashierSearchViewModel cashierSearchViewModel)
        {
            CashierSearchViewModel searchViewModel = new CashierSearchViewModel
            {
                CustomerID = 0,
                AccountID = 0
            };
            ViewData["provideSearchParams"] = null;
            long customerId = Convert.ToInt64(cashierSearchViewModel.CustomerID);
            long accountId = Convert.ToInt64(cashierSearchViewModel.AccountID);
            // Depending on the option selected, removing the validation for the
            // irrelevant property
            if(customerId == 0 && accountId == 0)
            {
                ModelState.Remove("CustomerID");
                ModelState.Remove("AccountID");
                ViewData["provideSearchParams"] = "Please provide a value";
                return View(searchViewModel);
            }
            if (customerId == 0)
                ModelState.Remove("CustomerID");
            else
                ModelState.Remove("AccountID");

            TempData["accountsExist"] = null;
            if (ModelState.IsValid)
            {
                if (customerId != 0)
                {
                    List<Account> accountSearchResults = _repo.GetAccountsByCustomer(customerId);
                    if (accountSearchResults.Count > 0)
                    {
                        searchViewModel.Accounts = accountSearchResults;
                    }
                }

                if (accountId != 0)
                {
                    Account accountSearchResult = _repo.GetAccountByAccountID(accountId);
                    if (accountSearchResult != null)
                    {
                        searchViewModel.Accounts = new List<Account>();
                        searchViewModel.Accounts.Add(accountSearchResult);
                    }
                }
                // If no accounts found, populate error message
                if (searchViewModel.Accounts == null)
                {
                    TempData["accountsExist"] = "No accounts found";
                }
                // Clear the input fields for a new search query
                ModelState.Clear();
                return View(searchViewModel);
            }

            return View(searchViewModel);
        }

        // Deposit Module
        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public IActionResult Deposit(string accountID)
        {
            long accountId = Convert.ToInt64(accountID);
            Account account = _repo.GetAccountByAccountID(accountId);
            CashierDepositViewModel cashierDepositViewModel = new CashierDepositViewModel
            {
                CustomerID = account.CustomerID,
                AccountID = account.AccountID,
                AccountType = account.AccountType,
                BalanceBeforeDeposit = account.Balance,
                LatestBalance = account.Balance,
                DepositAmount = 0
            };
            TempData["balanceBeforeDeposit"] = account.Balance.ToString();
            return View(cashierDepositViewModel);
        }


        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public IActionResult Deposit(CashierDepositViewModel cashierDepositViewModel)
        {
            TempData["depositMinBalance"] = null;
            TempData["depositMoney"] = null;
            if (ModelState.IsValid)
            {
                Account depositAccount = _repo.GetAccountByAccountID(cashierDepositViewModel.AccountID);
                if (depositAccount == null)
                {
                    TempData["depositMoney"] = $"Account {depositAccount} not found";
                    return View(cashierDepositViewModel);
                }
                TempData["balanceBeforeDeposit"] = depositAccount.Balance.ToString();
                depositAccount.Balance += cashierDepositViewModel.DepositAmount;
                depositAccount.LastUpdated = DateTime.Now;
                depositAccount.Duration = (depositAccount.LastUpdated.Date - depositAccount.CreateDate.Date).Days;

                Transaction depositTransaction = new Transaction
                {
                    CustomerID = depositAccount.CustomerID,
                    AccountType = depositAccount.AccountType,
                    TransactionType = "debit",
                    Amount = cashierDepositViewModel.DepositAmount,
                    TransactionDate = DateTime.Now,
                    SourceAccount = -1,
                    DestinationAccount = depositAccount.AccountID
                };

                _repo.Deposit(depositAccount, depositTransaction);

                CashierDepositViewModel updatedCashierDepositViewModel = new CashierDepositViewModel
                {
                    CustomerID = depositAccount.CustomerID,
                    AccountID = depositAccount.AccountID,
                    AccountType = depositAccount.AccountType,
                    BalanceBeforeDeposit = Convert.ToInt64(TempData["balanceBeforeDeposit"]),
                    LatestBalance = depositAccount.Balance,
                    DepositAmount = 0
                };

                if (depositAccount.Balance < 1000)
                {
                    TempData["depositMinBalance"] = $"Balance of account {depositAccount.AccountID} is below the minimum balance";
                }
                TempData["depositMoney"] = $"${cashierDepositViewModel.DepositAmount} deposited into {depositAccount.AccountID} successfully. Balance before deposit: ${updatedCashierDepositViewModel.BalanceBeforeDeposit} Latest balance: ${updatedCashierDepositViewModel.LatestBalance}";
                ModelState.Clear();
                Response.Redirect("/Cashier/Index");
            }
            return View(cashierDepositViewModel);
        }

        // Withdraw Module
        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public IActionResult Withdraw(string accountID)
        {
            long accountId = Convert.ToInt64(accountID);
            Account account = _repo.GetAccountByAccountID(accountId);
            CashierWithdrawViewModel cashierWithdrawViewModel = new CashierWithdrawViewModel
            {
                CustomerID = account.CustomerID,
                AccountID = account.AccountID,
                AccountType = account.AccountType,
                BalanceBeforeWithdrawal = account.Balance,
                LatestBalance = account.Balance,
                WithdrawalAmount = 0
            };
            TempData["balanceBeforeWithdrawal"] = account.Balance.ToString();
            return View(cashierWithdrawViewModel);
        }

        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public IActionResult Withdraw(CashierWithdrawViewModel cashierWithdrawViewModel)
        {
            TempData["withdrawMinBalance"] = null;
            TempData["withdrawMoney"] = null;
            if (ModelState.IsValid)
            {
                Account withdrawalAccount = _repo.GetAccountByAccountID(cashierWithdrawViewModel.AccountID);
                if (withdrawalAccount == null)
                {
                    TempData["withdrawMoney"] = $"Account {cashierWithdrawViewModel.AccountID} not found";
                    return View(cashierWithdrawViewModel);
                }

                withdrawalAccount.Balance -= cashierWithdrawViewModel.WithdrawalAmount;
                withdrawalAccount.LastUpdated = DateTime.Now;
                withdrawalAccount.Duration = (withdrawalAccount.LastUpdated.Date - withdrawalAccount.CreateDate.Date).Days;

                Transaction withdrawTransaction = new Transaction
                {
                    CustomerID = withdrawalAccount.CustomerID,
                    AccountType = withdrawalAccount.AccountType,
                    TransactionType = "credit",
                    Amount = cashierWithdrawViewModel.WithdrawalAmount,
                    TransactionDate = DateTime.Now,
                    SourceAccount = withdrawalAccount.AccountID,
                    DestinationAccount = -1
                };


                long balanceBeforeWithdrawal = Convert.ToInt64(TempData["balanceBeforeWithdrawal"]);
                _repo.Withdraw(withdrawalAccount, withdrawTransaction);
                CashierWithdrawViewModel updatedCashierWithdrawViewModel = new CashierWithdrawViewModel
                {
                    CustomerID = withdrawalAccount.CustomerID,
                    AccountID = withdrawalAccount.AccountID,
                    AccountType = withdrawalAccount.AccountType,
                    BalanceBeforeWithdrawal = balanceBeforeWithdrawal,
                    LatestBalance = balanceBeforeWithdrawal == 0 ? 0 : withdrawalAccount.Balance,
                    WithdrawalAmount = 0
                };

                if (withdrawalAccount.Balance < 1000)
                {
                    TempData["withdrawMinBalance"] = $"Balance of account {withdrawalAccount.AccountID} is below the minimum balance";
                }
                TempData["withdrawMoney"] = $"${cashierWithdrawViewModel.WithdrawalAmount} has been withdrawn from {withdrawalAccount.AccountID} successfully. Balance before withdrawal: ${updatedCashierWithdrawViewModel.BalanceBeforeWithdrawal} Latest balance: ${updatedCashierWithdrawViewModel.LatestBalance}";
                ModelState.Clear();
                Response.Redirect("/Cashier/Index");
            }
            return View(cashierWithdrawViewModel);
        }

        // Transfer Module
        [HttpGet]
        [Authorize(Roles = "Cashier")]
        public IActionResult Transfer(long accountId)
        {
            List<Account> emptyAccount = new List<Account>();
            TransferViewModel tvm = new TransferViewModel
            {
                SourceAccount = _repo.GetAccountByAccountID(accountId),
                Accounts=emptyAccount
            };
            return View(tvm);
        }
        
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public IActionResult Transfer(TransferViewModel transfer)
        {
            if ((transfer.TargetAccountID.ToString().Length < 9 || transfer.AmountToTransfer <= 0)&& transfer.TargetCustomerID != 0)
            {
                TransferViewModel tvm6 = new TransferViewModel
                {
                    SourceAccount = _repo.GetAccountByAccountID(transfer.SourceAccount.AccountID),
                    Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID),
                    Searched = true
                };
                if (tvm6.Accounts.Count == 0)
                {
                    TempData["NoAccounts"] = "No accounts found.";
                }
                return View(tvm6);
            }
            if (ModelState.IsValid)
            {
                if (transfer.SourceAccount.AccountID != transfer.TargetAccountID)
                {
                    Account targetAccount = _repo.GetAccountByAccountID(transfer.TargetAccountID);
                   
                    if (targetAccount != null)
                    {
                        if (targetAccount.AccountStatus.ToLower().Equals("pending"))
                        {
                            TempData["InactiveAccount"] = $"Cannot proceed with transfer as {targetAccount.AccountID} is inactive";
                            TransferViewModel tvm5 = new TransferViewModel
                            {
                                SourceAccount = transfer.SourceAccount,
                                Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID)
                            };
                            return View(tvm5);
                        }
                        if (transfer.SourceAccount.Balance >= transfer.AmountToTransfer)
                        {
                            transfer.SourceAccount.LastUpdated = DateTime.Now;
                            transfer.SourceAccount.Duration = (transfer.SourceAccount.LastUpdated - transfer.SourceAccount.CreateDate).Days;
                            targetAccount.LastUpdated = DateTime.Now;
                            targetAccount.Duration = (targetAccount.LastUpdated - targetAccount.CreateDate).Days;
                            transfer.SourceAccount.Balance -= transfer.AmountToTransfer;
                            targetAccount.Balance += transfer.AmountToTransfer;
                            Transaction transferSourceTransaction = new Transaction
                            {
                                CustomerID = transfer.SourceAccount.CustomerID,
                                AccountType = transfer.SourceAccount.AccountType,
                                TransactionType = "transfer",
                                Amount = transfer.AmountToTransfer,
                                TransactionDate = DateTime.Now,
                                SourceAccount = transfer.SourceAccount.AccountID,
                                DestinationAccount = targetAccount.AccountID
                            };


                            _repo.Transfer(transfer.SourceAccount, targetAccount, transferSourceTransaction);
                            TempData["TransferSuccess"] = "Transfer transaction was successful!";
                            if (transfer.SourceAccount.Balance < 1000)
                            {
                                TempData["SourceMinBalance"] = $"Balance of account {transfer.SourceAccount.AccountID} is below the minimum balance";
                            }
                            if (targetAccount.Balance < 1000)
                            {
                                TempData["TargetMinBalance"] = $"Balance of account {targetAccount.AccountID} is below the minimum balance";
                            }
                            return RedirectToAction("Index");
                        }
                        TempData["InvalidBalance"] = "Insufficient funds.";
                        TransferViewModel tvm2 = new TransferViewModel
                        {
                            SourceAccount = transfer.SourceAccount,
                            Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID)
                        };
                        return View(tvm2);
                    }
                    TempData["InvalidAccount"] = "No account matches this ID number.";
                    TransferViewModel tvm3 = new TransferViewModel
                    {
                        SourceAccount = transfer.SourceAccount,
                        Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID)
                    };
                    return View(tvm3);
                }
                TempData["InvalidAccounts"] = "Cannot be the same as the source account.";
                TransferViewModel tvm4 = new TransferViewModel
                {
                    SourceAccount = transfer.SourceAccount,
                    Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID)

                };
                return View(tvm4);
            }
            TransferViewModel tvm = new TransferViewModel
            {
                SourceAccount = transfer.SourceAccount,
                Accounts = _repo.GetAccountsByCustomer(transfer.TargetCustomerID)

            };
            return View(tvm);
        }

        [Authorize(Roles = "Cashier")]
        [HttpGet]
        public IActionResult ViewStatement(long ID, int? PageIndex, int? PageSize, int? Count, DateTime? FromDate, DateTime? ToDate)
        {
            if (ModelState.IsValid)
            {
                int pageIndex = PageIndex ?? 1;
                int pageSize = PageSize ?? 5;
                DateTime toDate = ToDate ?? DateTime.Now;
                DateTime fromDate = FromDate ?? toDate.AddDays(-30);
                int count = Count ?? _repo.CountTransactions(ID, fromDate, toDate);
                if (count == 0)
                {
                    return View(PagingList<AccountStatement>.CreateEmpty());
                }
                int lastRecordNum = pageSize * pageIndex;
                int firstRecordNum = lastRecordNum - pageSize + 1;
                int effectivePageSize = pageSize;
                if (lastRecordNum > count)
                {
                    effectivePageSize = count - firstRecordNum + 1;
                }

                return View(_repo.ViewStatement(ID, pageIndex, pageSize, effectivePageSize, count, fromDate, toDate));
            }
            return View();
        }
    }
}
