using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
 public  class CustomerPaymentPhoto
    {
        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }

        [ForeignKey("CustomerPayment")]
        public Guid CustomerPaymentId { get; set; }
        public CustomerPayment CustomerPayment { get; set; }

    }
}
