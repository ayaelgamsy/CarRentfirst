using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Car> Cars { get; }
        IRepository<CarOwner> CarOwners { get; }
        IRepository<CarOwnerPayment> CarOwnerPayments { get; }
        IRepository<CarOwnerAccount> CarOwnerAccounts { get; }
        IRepository<CashDeposit> CashDeposits { get; }
        IRepository<Cashwithdrawal> Cashwithdrawals { get; }
        IRepository<Company> Companys { get; }
        IRepository<Customer> Customers { get; }
        IRepository<CustomerAccount> CustomerAccounts { get; }
        IRepository<CustomerPayment> CustomerPayments { get; }
        IRepository<CustomerRent> CustomerRents { get; }
        IRepository<Employee> Employees { get; }
        IRepository<Expense> Expenses { get; }
        IRepository<ExpenseType> ExpenseTypes { get; }
        IRepository<Government> Governments { get; }
        IRepository<OwnerRentContract> OwnerRentContracts { get; }
        IRepository<Role> Roles { get; }
        IRepository<RoleClaims> RoleClaimss { get; }
        IRepository<Stock> Stocks { get; }
        IRepository<StockMovement> StockMovements { get; }
        IRepository<StockTransfer> StockTransfers { get; }
        IRepository<User> Users { get; }
        IRepository<UserRole> UserRoles { get; }
        
       




        int Complete();
    }
}
