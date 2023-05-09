using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.RolesDto
{
    public class RolesRegisterDto
    {
        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
    }
}
