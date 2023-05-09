using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public class OwnerRentContract : EntityBase
    {

        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerMonth { get; set; }
        
        public double NumberOfMonths { get; set; }

        public double TotalValue  { get; set; }

        public double Payment  { get; set; }
       
        public double RestValue  { get; set; }

        public string Notes { get; set; }



        [ForeignKey("CarOwner")]
        public Guid CarOwnerId { get; set; }
        public CarOwner CarOwner { get; set; }

        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }

    


    }
}
