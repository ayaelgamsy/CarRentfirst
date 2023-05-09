using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class CashDeposit : EntityBaseName
    {
        //ايداع
        public double Value { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }
    }
}
