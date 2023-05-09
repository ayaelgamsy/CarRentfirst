using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerAccountDto
{
    public class CustomerAccountRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public Guid CustomerId { get; set; }
        public IEnumerable<Customer> Customers { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ToDate { get; set; }
    }
}
