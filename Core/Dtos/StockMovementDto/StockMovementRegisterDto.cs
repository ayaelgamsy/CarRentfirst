using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockMovementDto
{
    public class StockMovementRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<Stock> Stocks { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ToDate { get; set; }
      
    }
}
