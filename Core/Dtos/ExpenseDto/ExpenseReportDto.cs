using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ExpenseDto
{
    public class ExpenseReportDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid ExpenseTypeId { get; set; }
        public List<DrpDto> DrpExpenseTypeDto { get; set; }


        [Required(ErrorMessage = "*")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ToDate { get; set; }
    }
}
