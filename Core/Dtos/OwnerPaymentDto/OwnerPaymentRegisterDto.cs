using Core.Dtos.ExpenseDto;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerPaymentDto
{
    public class OwnerPaymentRegisterDto
    {
        public Guid? Id { get; set; }
        public double? CurrentDebt { get; set; }

        [Required(ErrorMessage = "*")]
        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? Value { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarOwnerId { get; set; }
        public IEnumerable<DrpDto> CarOwners { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<DrpDto> Stocks { get; set; }
    }
}
