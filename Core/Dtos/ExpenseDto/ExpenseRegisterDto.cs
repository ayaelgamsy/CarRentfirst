using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Dtos.ExpenseDto
{
  public  class ExpenseRegisterDto
    {
        public Guid ?  Id { get; set; }

        [Required(ErrorMessage ="*")]
        public DateTime ?  Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public double ?  Value { get; set; }

        [Required(ErrorMessage = "*")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid ExpenseTypeId { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }

        public List<DrpDto> DrpExpenseTypeDto { get; set; }
        public List<DrpDto> DrpstockDto { get; set; }

    }
}
