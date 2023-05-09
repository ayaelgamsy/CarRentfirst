using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.GovernmentDto
{
    public class GovernmentRegisterDto
    {
        public Guid? Id { get; set; }
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(2)]
        public string Code { get; set; }
    }
}
