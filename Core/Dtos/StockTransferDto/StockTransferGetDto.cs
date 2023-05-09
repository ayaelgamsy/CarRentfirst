using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockTransferDto
{
  public  class StockTransferGetDto
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }

        public string StockFromName { get; set; }

        public string ToStockName { get; set; }


    }
}
