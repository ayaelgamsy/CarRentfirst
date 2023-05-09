using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
 public   class CarOwnerPayment : EntityBase
    {

        public double CurrentDebt { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }


        [ForeignKey("CarOwner")]
        public Guid CarOwnerId { get; set; }
        public CarOwner CarOwner { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }


    }
}
