using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAvailableByDateDto
{
    public class CarAvailableByDateRegisterDto
    {
        public Guid? Id { get; set; } 

        [Required(ErrorMessage = "*")]
        public DateTime? EndDate { get; set; }
    }
}
