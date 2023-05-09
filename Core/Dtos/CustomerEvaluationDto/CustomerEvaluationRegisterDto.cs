using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerEvaluationDto
{
    public class CustomerEvaluationRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public string Name { get; set; }
    }
}
