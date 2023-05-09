using Core.Entities;
using Core.Interfaces;
using Infrastracture.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Services
{
     class UnitOfWork : IUnitOfWork
    {
        private readonly SiteDataContext _context;
        

        public IRepository<Car> Cars { get; private set; }
        public IRepository<CarOwner> CarOwners { get; private set; }
        public IRepository<CarOwnerPayment> CarOwnerPayments { get; private set; }
        public IRepository<CarOwnerAccount> CarOwnerAccounts { get; private set; }
        public IRepository<CashDeposit> CashDeposits { get; private set; }
        public IRepository<Cashwithdrawal> Cashwithdrawals { get; private set; }
        public IRepository<Company> Companys { get; private set; }
        public IRepository<Customer> Customers { get; private set; }
        public IRepository<CustomerAccount> CustomerAccounts { get; private set; }
        public IRepository<CustomerPayment> CustomerPayments { get; private set; }
        public IRepository<CustomerRent> CustomerRents { get; private set; }
        public IRepository<Employee> Employees { get; private set; }
        public IRepository<Expense> Expenses { get; private set; }
        public IRepository<ExpenseType> ExpenseTypes { get; private set; }
        public IRepository<Government> Governments { get; private set; }
        public IRepository<OwnerRentContract> OwnerRentContracts { get; private set; }
        public IRepository<Role> Roles { get; private set; }
        public IRepository<RoleClaims> RoleClaimss { get; private set; }
        public IRepository<Stock> Stocks { get; private set; }
        public IRepository<StockMovement> StockMovements { get; private set; }
        public IRepository<StockTransfer> StockTransfers { get; private set; }
        public IRepository<User> Users { get; private set; }
        public IRepository<UserRole> UserRoles { get; private set; }


        public UnitOfWork(SiteDataContext context)
        {
            _context = context;
           
            Cars = new Repository<Car>(_context);
            CarOwners = new Repository<CarOwner>(_context);
            CarOwnerPayments = new Repository<CarOwnerPayment>(_context);
            CarOwnerAccounts = new Repository<CarOwnerAccount>(_context);
            CashDeposits = new Repository<CashDeposit>(_context);
            Cashwithdrawals = new Repository<Cashwithdrawal>(_context);
            Companys = new Repository<Company>(_context);
            Customers = new Repository<Customer>(_context);
            CustomerAccounts = new Repository<CustomerAccount>(_context);
            CustomerPayments = new Repository<CustomerPayment>(_context);
            CustomerRents = new Repository<CustomerRent>(_context);
            Employees = new Repository<Employee>(_context);
            Expenses = new Repository<Expense>(_context);
            ExpenseTypes = new Repository<ExpenseType>(_context);
            Governments = new Repository<Government>(_context);
            OwnerRentContracts = new Repository<OwnerRentContract>(_context);
            Roles = new Repository<Role>(_context);
            RoleClaimss = new Repository<RoleClaims>(_context);
            Stocks = new Repository<Stock>(_context);
            StockMovements = new Repository<StockMovement>(_context);
            StockTransfers = new Repository<StockTransfer>(_context);
            Users = new Repository<User>(_context);
            UserRoles = new Repository<UserRole>(_context);


        }
        

        public int Complete()
        {
            return _context.SaveChanges();

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
