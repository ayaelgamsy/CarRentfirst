using Core.Dtos.ExpenseDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockTransferDto
{
  public  class StockTransferRegisterDto
    {
        public Guid ?  Id { get; set; }

        [Required(ErrorMessage = "*")]
        public double ? Value { get; set; }

        [Required(ErrorMessage = "*")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public Guid FromStockId { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid ToStockId { get; set; }


        public List<DrpDto> DrpstockDto { get; set; }
    }
}
