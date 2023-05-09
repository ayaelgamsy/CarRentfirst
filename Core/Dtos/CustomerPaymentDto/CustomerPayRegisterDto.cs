using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerPaymentDto
{
    public class CustomerPayRegisterDto
    {
        public Guid? Id { get; set; }

       // [Required(ErrorMessage ="*")]
        public double? CurrentDebt { get; set; }

        [Required(ErrorMessage = "*")]
        public double? Value { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;
        public string Notes { get; set; }


        [Required(ErrorMessage = "*")]
        public Guid CustomerId { get; set; }
        public IEnumerable<Customer> Customers { get; set; }


        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }
    }
}
