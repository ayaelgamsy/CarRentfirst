using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerPaymentDto
{
    public class CustomerPayGetDto
    {
        public Guid Id { get; set; }
        public double CurrentDebt { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }


        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }

        public Guid StockId { get; set; }
        public string StockName { get; set; }
    }
}
