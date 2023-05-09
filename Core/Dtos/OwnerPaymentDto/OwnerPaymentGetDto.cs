using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerPaymentDto
{
    public class OwnerPaymentGetDto
    {
        public Guid Id { get; set; }
        public string CarOwnerName { get; set; }
        public string StockName { get; set; }
        public double CurrentDebt { get; set; }
        public double Value { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }
}
