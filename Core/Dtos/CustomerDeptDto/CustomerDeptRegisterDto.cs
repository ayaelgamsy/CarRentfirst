using Core.Dtos.ExpenseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDeptDto
{
    public class CustomerDeptRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public Guid CustomerId { get; set; }
        public List<DrpDto> Customers { get; set; }

        [Required(ErrorMessage = "*")]
        public double? DebtValue { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;
        public string Notes { get; set; }


      
    }
}
