using Core.Common;
using Core.Common.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class CustomerRent : EntityBase
    {
        public string GuarantorName { get; set; }
        public List<GuarantorPhoto> GuarantorPhotos { get; set; }
        public Guid CustomerId { get ; set; }
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }
        public DateTime StartDate { get; set; }
        public DayType TypeOfDay { get; set; }
        public DateTime EndDate { get; set; }
        public double NumberOfDays { get; set; }
        public double PricePerDay { get; set; }
        public double Total { get; set; }
        public double payment { get; set; }
        public double RentDiscount { get; set; }
        public double RestValue { get; set; }
        public double CounterStart { get; set; }
        public double CounterEnd { get; set; }
        public double AllowedDistance { get; set; }
        public DateTime ActualBackDate { get; set; }
        public double LateHours { get; set; }
        public double PriceLatePerKm { get; set; }
        public double PriceTimeLatePerHoure { get; set; }
        public double FinalTotalAfterLate { get; set; }
        public double PaymentFinal { get; set; }
        public double RentBackDiscount { get; set; }
        public double RentBackRest { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; }
        public bool Finished { get; set; }
        public double PriceWash { get; set; }
        public double PetrolPrice { get; set; }
        public double IncreaseDistanc { get; set; }
        public string SerialNumber { get; set; }


        [ForeignKey("CustomerEvaluation")]
        public Guid? CustomerEvaluationId { get; set; }
        public CustomerEvaluation CustomerEvaluation { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }

        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public Guid? EmployeeReceiveId { get; set; }
        public Employee EmployeeReceive { get; set; }

        [ForeignKey("Marketer")]
        public Guid? MarketerId { get; set; }
        public Marketer Marketer { get; set; }
        public Guid? StockBackId { get; set; }
        public Stock StockBack { get; set; }


        
    }
}
