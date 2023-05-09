using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarStatusReportDto
{
    public class CarStatusGetDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string CarName { get; set; }
        public bool CarActive { get; set; }
        public bool CarAvailable { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double NumberOfDays { get; set; }
        public double PricePerDay { get; set; }
        public double Total { get; set; }
        public double payment { get; set; }
        public double RestValue { get; set; }
        public double CounterStart { get; set; }
        public double CounterEnd { get; set; }
        public double AllowedDistance { get; set; }
        public DateTime ActualBackDate { get; set; }
        public double LateHours { get; set; }
        public double PriceLatePerKm { get; set; }
        public double PriceTimeLatePerHoure { get; set; }
        public double FinalTotalAfterLate { get; set; }
        public double PaymentFinal { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; }
        public bool Finished { get; set; }
        public double PriceWash { get; set; }
        public double IncreaseDistanc { get; set; }
    }
}
