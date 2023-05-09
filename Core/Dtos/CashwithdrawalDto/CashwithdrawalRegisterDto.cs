using Core.Dtos.ExpenseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CashwithdrawalDto
{
  public  class CashwithdrawalRegisterDto
    {

        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        public double? Value { get; set; }

        [Required(ErrorMessage = "*")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }


        public List<DrpDto> DrpstockDto { get; set; }

    }
}
