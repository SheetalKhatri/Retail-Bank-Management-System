using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetailBankManagementSystem.Models;
using RetailBankManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RetailBankManagementSystem.Controllers
{
    [Authorize(Roles = "CustomerExecutive")]
    public class ExecutiveController : Controller
    {
        private List<string> states = new List<string> { "FL", "IL", "WI" };
        private List<string> cities = new List<string> { "Miami", "Fort Lauderdale", "Boca Raton", "Chicago", "Naperville", "Springfield", "Madisson", "Milwakee", "Green Bay" };

        private ICustomerExecutiveRepo _repo;

        public ExecutiveController(ICustomerExecutiveRepo repo)
        {
            _repo = repo;
        }

        public IActionResult ViewAccountsIndex(ViewAccountViewModel viewacc)
        {
            long? accountID = ModelState.IsValid ? viewacc.AccountID : null;
            viewacc.Accounts = _repo.ViewAccounts(viewacc.PageIndex ?? 1, accountID);
            return View(viewacc);
        }

        
        public IActionResult DeleteAccount(long ID, String? flag)
        {
            TempData["redirect"] = null;
            if (flag == "true")
            {
                TempData["redirect"] = "true";
            }
            Account account = _repo.GetAccountByAccountID(ID);
            return View(account);
        }

        [HttpPost]
        public IActionResult DeleteAccount(Account account)
        {
            long customerId = account.CustomerID;
            TempData["accountdeletion"] = null;
            if (account == null)
            {
                TempData["accountdeletion"] = "Account doesn't exist";
                return RedirectToAction("ViewAccountsIndex");

            }
            else
            {
                _repo.DeleteAccount(account);               
                String rand = TempData["redirect"] == null ? "" : TempData["redirect"].ToString();
                if (rand.Equals("true"))
                {
                    TempData["redirect"] = null;
                    return RedirectToAction("DeleteCustomer", new { CustomerID = customerId });
                } else
                {
                    TempData["accountdeletion"] = "Account " + account.AccountID + " is deleted successfully";
                    return RedirectToAction("ViewAccountsIndex");
                }
                
            }
        }
        public IActionResult Index(int? pageNumber)
        {
            //return View(_repo.ViewAllCustomers(PageIndex ?? 1));
            var customerList = _repo.ViewAllCustomers();
            int pageSize = 5;
            return View(PagingList<Customer>.CreateAsync(
                customerList.AsQueryable<Customer>(),
                pageNumber ?? 1,
                pageSize));
        }

        public IActionResult Create()
        {
           
            CustomerCreateViewModel cvm = new CustomerCreateViewModel
            {
                States = states,
                Cities = cities
            };
            return View(cvm);
        }
        [HttpPost]
        public IActionResult Create(CustomerCreateViewModel customer)
        {
            
            if (ModelState.IsValid)
            {
                Customer cs = _repo.getCustomerBySsn(Convert.ToInt64(customer.CustomerDetails.SSN));
                if (cs == null)
                {
                    customer.CustomerDetails.Address = customer.AddressLine1 + ", " + customer.AddressLine2;
                    _repo.AddCustomer(customer.CustomerDetails);
                    TempData["customerAddSuccess"] = $"Customer was added successfully! Customer ID: {customer.CustomerDetails.CustomerID}";
                    return RedirectToAction("Index");
                }
                TempData["customerAddFail"] = $"Customer could not be added. SSN is already linked to another customer account";
                return RedirectToAction("Index");
            }
          
            CustomerCreateViewModel cvm = new CustomerCreateViewModel
            {
                States = states,
                Cities = cities
            };
            return View(cvm);
        }
        
        
        public IActionResult viewStatus(int pageNo)
        {
            if(pageNo == 0)
            {
                pageNo = 1;
            }
            CustomerAccViewModel acc = new CustomerAccViewModel();
            acc.customerAccs = _repo.viewStatus().customerAccs;
            return View(PagingList<CustomerAcc>.CreateAsync(acc.customerAccs.AsQueryable<CustomerAcc>(), pageNo, 5));
        }

        public IActionResult SearchCustomer()
        {
            TempData["customer"] = null;
            return View();
        }

        //[HttpPost]
        //public IActionResult SearchCustomer(Customer customer)
        //{
        //    if (customer.CustomerID == 0)
        //        ModelState.Remove("CustomerID");
        //    else
        //        ModelState.Remove("SSN");

        //    List<Customer> cs = new List<Customer>();
        //    TempData["customer"] = null;
        //        if (customer.SSN == 0)
        //        {
        //            cs.Add(_repo.getCustomerByCID(customer.CustomerID));

        //            if (!cs.Contains(null))
        //            {
        //                TempData["customer"] = "we have a value";
        //            }
        //            return View(cs.First());
        //        }
        //        else
        //        {
        //            cs.Add(_repo.getCustomerBySsn(customer.SSN));
        //            if (!cs.Contains(null))
        //            {
        //                TempData["customer"] = "we have a value";
        //            }
        //            return View(cs.First());
        //        }
        //}


        [HttpPost]
        public IActionResult SearchCustomer(CustSearch customer)
        {
            if (customer.CustomerID == 0)
                ModelState.Remove("CustomerID");
            else
                ModelState.Remove("SSN");
            long customerId = customer.CustomerID;
            long ssn = customer.SSN;

            ViewData["SearchParams"] = null;
            if (customerId == 0 && ssn == 0)
            {
                ModelState.Remove("CustomerID");
                ModelState.Remove("SSN");
                ViewData["SearchParams"] = "Please Provide A Value!";
                return View("SearchCustomer");
            }

            List<Customer> cs = new List<Customer>();
            TempData["customer"] = null;
            if (!ModelState.IsValid)
            {
                return View("SearchCustomer");
            }
            if (customer.SSN == 0)
            {
                cs.Add(_repo.getCustomerByCID(customer.CustomerID));

                if (cs.Contains(null))
                {
                    TempData["customer"] = "we have a null value";
                    return View();
                } else
                {

                    return View("viewCustomerDetails", cs.First());
                }                
            }
            else
            {
                cs.Add(_repo.getCustomerBySsn(customer.SSN));
                if (cs.Contains(null))
                {
                    TempData["customer"] = "we have a null value";
                    return View();
                }
                else
                {
                    return View("viewCustomerDetails", cs.First());
                }
            }
        }

        public IActionResult viewCustomerDetails(Customer customer)
        {            
            return View(customer);
        }

        public IActionResult viewAccountDetails(long CustomerID)
        {
            return View(_repo.GetAccountByCustomerID(CustomerID));
        }

        [HttpGet]
        public IActionResult DeleteCustomer(long CustomerID)
        {
            Console.WriteLine(CustomerID);
            TempData["account"] = null;
            CustomerAccDeleteModel customerAccDeleteModel = new CustomerAccDeleteModel();
            customerAccDeleteModel.customer = _repo.getCustomerByCID(CustomerID);
            customerAccDeleteModel.accounts = _repo.GetAllAccountsByCustomerID(CustomerID);            
            if(customerAccDeleteModel.accounts.Count != 0)
            {
                TempData["account"] = "exist";
            }
            return View(customerAccDeleteModel);
        }

        [HttpPost]
        public IActionResult DeleteCustomer(Customer customer)
        {
            TempData["account"] = "Customer "+customer.CustomerID + " has been deleted";
            _repo.DeleteCustomer(customer);            
            return RedirectToAction("Index");
        }

        public IActionResult CreateAccount(long CustomerID)
        {
            List<Account> accounts = _repo.GetAllAccountsByCustomerID(CustomerID);
            //TempData["exist"] = null;
            if (accounts.Count == 2) 
            {
                TempData["exist"] = "User already has Checking and Savings account!";
                return View();
            } else if(accounts.Count == 1)
            {
                return View(accounts.First());
            }
            Account account = new Account();
            account.CustomerID = CustomerID;
            return View(account);
        }

        [HttpPost]
        public IActionResult CreateAccount(Account account)
        {
            TempData["account"] = null;
            TempData["accountCreated"] = null;
            List<Account> accounts = _repo.GetAllAccountsByCustomerID(account.CustomerID);
            if (account.Balance < 1000)
            {
                TempData["account"] = "Provided balance is below the minimum balance";
                return View(account);
            } 
            foreach(Account acc in accounts)
            {
                if(acc.AccountType.ToLower() == account.AccountType.ToLower())
                {
                    TempData["account"] = "Selected account type already exists";
                    return View(account);
                }
            }
            account.AccountStatus = "Pending";
            account.CreateDate = DateTime.Today;
            account.LastUpdated = DateTime.Today;
            account.Duration = 0;
            _repo.AddAccount(account);
            TempData["accountCreated"] = $"{account.AccountType} account created successfully for Customer {account.CustomerID}";
            return RedirectToAction("viewAccountDetails", new { CustomerID = account.CustomerID });
        }
        public IActionResult UpdateCustomer(long customerID)
        {
            Customer customer = _repo.getCustomerByCID(customerID);
            CustomerUpdateViewModel cs = new CustomerUpdateViewModel
            {
                CustomerDetails=customer,
                Address=customer.Address,
                States = states,
                Cities = cities
            };
            return View(cs);
        }
        [HttpPost]
        public IActionResult UpdateCustomer(CustomerUpdateViewModel customer)
        {
            if (ModelState.IsValid)
            {
                customer.CustomerDetails.Address = customer.Address;
                _repo.EditCustomer(customer.CustomerDetails);
                TempData["customerUpdateSuccess"] = "Customer was updated successfully!";
                return RedirectToAction("Index");
            }
            Customer cs = _repo.getCustomerByCID(customer.CustomerDetails.CustomerID);
            CustomerUpdateViewModel customerUpdate = new CustomerUpdateViewModel
            {
                CustomerDetails = cs,
                Address = cs.Address,
                States = states,
                Cities = cities
            };
            return View(customerUpdate);
        }

        [HttpPost]
        public IActionResult ActivateAccount(Account account, int? PageIndex)
        {
            TempData["accountactivation"] = null;
            if (account == null)
            {
                TempData["accountactivation"] = "Account doesn't exist";
                return RedirectToAction("ViewAccountsIndex");

            }
            else
            {
                _repo.ActivateAccount(account);
                TempData["accountactivation"] = "Account " + account.AccountID + " is activated successfully";

                return RedirectToAction("ViewAccountsIndex", new { PageIndex = PageIndex??1});
            }
        }

    }
}
