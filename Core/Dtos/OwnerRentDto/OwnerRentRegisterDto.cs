using Core.Dtos.ExpenseDto;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerRentDto
{
    public class OwnerRentRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? EndDate { get; set; }

        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? PricePerMonth { get; set; }

        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? NumberOfMonths { get; set; }

        public double TotalValue { get; set; }

        [RegularExpression(@"(\d+)?(\.)?(\d+)?", ErrorMessage = "No Char")]
        public double? Payment { get; set; }

        public double RestValue { get; set; }

        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarOwnerId { get; set; }
        public IEnumerable<DrpDto> CarOwners { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarId { get; set; }
        public IEnumerable<DrpDto> Cars { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<DrpDto> Stocks { get; set; }
    }
}
