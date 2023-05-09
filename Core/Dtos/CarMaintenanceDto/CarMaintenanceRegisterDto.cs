using Core.Dtos.ExpenseDto;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarMaintenanceDto
{
    public class CarMaintenanceRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public Guid CarId { get; set; }
        public List<DrpDto> Cars { get; set; }

        public DateTime? date { get; set; } = DateTime.Now;
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public double? value { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public List<DrpDto> Stocks { get; set; }
    }
}
