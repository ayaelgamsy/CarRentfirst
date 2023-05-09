using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerReservationDto
{
    public class CustomerReservationGetDto
    {
        public Guid Id { get; set; }

        public string Customer { get; set; }
        public string CarName { get; set; }
        public string MarketerName { get; set; }

        public DateTime Date { get; set; } 
        public DateTime StartDate { get; set; }
        public double NumberOfDays { get; set; }
        public DateTime EndDate { get; set; }
        public double AllowedDistance { get; set; }
        public double Value { get; set; }
        public List<CustomerReservationPhoto> CustomerReservationPhotos { get; set; }
        public string Notes { get; set; }

    }
}
