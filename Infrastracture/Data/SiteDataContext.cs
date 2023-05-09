using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Data
{
   public class SiteDataContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, RoleClaims, IdentityUserToken<Guid>>

    {

        public SiteDataContext(DbContextOptions<SiteDataContext> options) : base(options)
        { }

        public DbSet<Government> governments { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarPhoto> CarPhotos { get; set; }
        public DbSet<CarVideo> CarVideos { get; set; }
        public DbSet<CarOwner> CarOwners { get; set; }
        public DbSet<CarOwnerPhoto> CarOwnerPhotos { get; set; }
        public DbSet<CarOwnerAccount> CarOwnerAccounts { get; set; }
        public DbSet<CarOwnerPayment> CarOwnerPayments { get; set; }
        public DbSet<CashDeposit> CashDeposits { get; set; }
        public DbSet<Cashwithdrawal> Cashwithdrawals { get; set; }
        public DbSet<Company> Companys { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerPhoto> CustomerPhotos { get; set; }
        public DbSet<CustomerAccount> CustomerAccounts { get; set; }
        public DbSet<CustomerPayment> CustomerPayments { get; set; }
        public DbSet<CustomerRent> CustomerRents { get; set; }
        public DbSet<GuarantorPhoto> GuarantorPhotos { get; set; }
        public DbSet<CustomerEvaluation> CustomerEvaluations { get; set; }
        public DbSet<CustomerReservation> CustomerReservations { get; set; }


        public DbSet<CustomerViolation> CustomerViolations { get; set; }
        public DbSet<CustomerViolationPhoto> CustomerViolationPhotos { get; set; }





        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeePhoto> EmployeePhotos { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<OwnerRentContract> OwnerRentContracts { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<StockMovement> StockMovements { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
      

        public DbSet<CarAccident> CarAccidents { get; set; }
        public DbSet<CarAccidentPhoto> CarAccidentPhotos { get; set; }
        public DbSet<CarAccidentVideo> CarAccidentVideos { get; set; }


        public DbSet<CarMaintenance> CarMaintenances { get; set; }
        public DbSet<CustomerLastDept> CustomerLastDepts { get; set; }
         public DbSet<CustomerPaymentPhoto> CustomerPaymentPhotos { get; set; }
         public DbSet<Marketer> Marketers { get; set; }
         public DbSet<MarketerPhoto> MarketerPhotos { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StockMovement>()
                .Property(p => p.DifferentValue)
                .HasComputedColumnSql("[InValue]-[OutValue]");


            //modelBuilder.Entity<StockMovement>()
            //   .Property(p => p.Ckeck)
            //   .HasDefaultValue(false);
               


            // relation on to many

            modelBuilder.Entity<Stock>()
                             .HasMany(c => c.StockTransfersto)
                             .WithOne(e => e.ToStock).HasForeignKey(ur => ur.ToStockId);


            //one to many EmployeeReceive and CustomerRent

            modelBuilder.Entity<Employee>()
                            .HasMany(c => c.CustomerRentReceives)
                            .WithOne(e => e.EmployeeReceive).HasForeignKey(ur => ur.EmployeeReceiveId);

            //one to many marketer and CustomerRent

            //modelBuilder.Entity<Employee>()
            //                .HasMany(c => c.CustomerRentmarketer)
            //                .WithOne(e => e.Marketer).HasForeignKey(ur => ur.MarketerId);


            ////rentback stockmovement
            //modelBuilder.Entity<CustomerRent>()
            //                .HasMany(c => c.stockMovements)
            //                .WithOne(e => e.CustomerRentBack).HasForeignKey(ur => ur.CustomerRentBackId);

            ////rentback customeraccount
            //modelBuilder.Entity<CustomerRent>()
            //                .HasMany(c => c.customerAccounts)
            //                .WithOne(e => e.CustomerRentBack).HasForeignKey(ur => ur.CustomerRentBackId);



            //rentback stock
            modelBuilder.Entity<Stock>()
                            .HasMany(c => c.CustomerBackRents)
                            .WithOne(e => e.StockBack).HasForeignKey(ur => ur.StockBackId);
            

            // primary key
            modelBuilder.Entity<IdentityUserLogin<Guid>>().HasNoKey();
            modelBuilder.Entity<IdentityUserToken<Guid>>().HasNoKey();
            modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserId, ur.RoleId });

           // relation many to many
            modelBuilder.Entity<UserRole>().HasOne(ur => ur.Role)
             .WithMany(r => r.UserRole).HasForeignKey(ur => ur.RoleId)
             .IsRequired();
            modelBuilder.Entity<UserRole>().HasOne(ur => ur.User)
            .WithMany(r => r.UserRole).HasForeignKey(ur => ur.UserId)
            .IsRequired();
        }

    }
}
