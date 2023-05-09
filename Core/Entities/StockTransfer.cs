using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class StockTransfer : EntityBase
    {


        public double Value { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("StockFrom")]
        public Guid FromStockId { get; set; }
        public Stock StockFrom { get; set; }



        public Guid ToStockId { get; set; }
        public Stock ToStock { get; set; }

    }
}
