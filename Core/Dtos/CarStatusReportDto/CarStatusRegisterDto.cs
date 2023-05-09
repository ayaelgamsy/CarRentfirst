using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarStatusReportDto
{
   public class CarStatusRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarId { get; set; }
        public IEnumerable<Car> Cars { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? FromDate { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ToDate { get; set; }
   
    }
}
