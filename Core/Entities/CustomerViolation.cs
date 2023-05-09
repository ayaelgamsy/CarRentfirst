using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerViolation:EntityBase
    {
        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime ViolationDate { get; set; }
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double Payment { get; set; }
        public double Rest { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }

        public string ViolationNumber { get; set; }
        public string ViolationState { get; set; }
        public string ViolationPlace { get; set; }
        public List<CustomerViolationPhoto> CustomerViolationPhotos { get; set; }
        public string Notes { get; set; }
    }
}
