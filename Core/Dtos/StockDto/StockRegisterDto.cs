using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockDto
{
    public class StockRegisterDto
    {
        public Guid ? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public string Name { get; set; }

        [Required(ErrorMessage ="*")]
        public double ? StartAccount { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;

        public List<StockGetDto> StockGetDto { get; set; }



    }
}
