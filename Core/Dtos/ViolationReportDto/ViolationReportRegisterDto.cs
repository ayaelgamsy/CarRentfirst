using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ViolationReportDto
{
    public class ViolationReportRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarId { get; set; }
        public IEnumerable<Car> Cars { get; set; }
    }
}
