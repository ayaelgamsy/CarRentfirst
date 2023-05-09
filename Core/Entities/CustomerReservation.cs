using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerReservation:EntityBase
    {
        
        public string Customer { get; set; }

        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }


        [ForeignKey("Marketer")]
        public Guid MarketerId { get; set; }
        public Marketer Marketer { get; set; }

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
