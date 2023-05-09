using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CashwithdrawalDto
{
   public class CashwithdrawalGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public string Notes { get; set; }
        public DateTime Date { get; set; }
        public string StockName { get; set; }

    }
}
