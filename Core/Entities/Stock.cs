using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class Stock : EntityBaseName
    {
       
        public double StartAccount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
      
        public List<Expense> Expenses { get; set; }
        public List<Cashwithdrawal> Cashwithdrawals { get; set; }
        public List<CashDeposit> CashDeposits { get; set; }
        public List<StockTransfer> StockTransfers { get; set; }
        public List<OwnerRentContract> OwnerRentContracts { get; set; }
        public List<CarOwnerPayment> CarOwnerPayments { get; set; }
        public List<CustomerPayment> CustomerPayments { get; set; }
        public List<CustomerRent> CustomerRents { get; set; }
        public List<StockTransfer> StockTransfersto { get; set; }
        public List<StockMovement> StockMovement { get; set; }
        public List<CarAccident> CarAccidents { get; set; }
        public List<CustomerRent> CustomerBackRents { get; set; }
        public List<CarMaintenance> CarMaintenances { get; set; }
        

    }
}
