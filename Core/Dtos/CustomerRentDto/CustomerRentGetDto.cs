using Core.Entities;
using System;
using System.Collections.Generic;

namespace Core.Dtos.CustomerRentDto
{
    public class CustomerRentGetDto
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; }
        public string CarName { get; set; }
        public string EmployeeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double NumberOfDays { get; set; }
        public double Total { get; set; }
        public double Payment { get; set; }
        public double RestValue { get; set; }
        public double CounterStart { get; set; }
        public string SerialNumber { get; set; }

        public List<GuarantorPhoto> GuarantorPhotos { get; set; }
    }
}
