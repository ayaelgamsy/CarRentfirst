using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public class CustomerLastDept : EntityBase
    {
        public double DebtValue { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }


        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }


    }
}
