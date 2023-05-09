
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerRentDto
{
   public class OwnerRentGetDto
    {
        public Guid Id { get; set; }
        public string CarOwnerName { get; set; }
        public string CarName { get; set; }
        public string StockName { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double PricePerMonth { get; set; }

        public double NumberOfMonths { get; set; }

        public double TotalValue { get; set; }

        public double Payment { get; set; }

        public double RestValue { get; set; }

        public string Notes { get; set; }
    }
}
