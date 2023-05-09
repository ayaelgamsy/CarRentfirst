using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
  public  class CarMaintenance : EntityBase
    {

        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public  Car Car { get; set; }

        public DateTime date { get; set; }
        public string Notes { get; set; }
        public double value { get; set; }
        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }



    }
}
