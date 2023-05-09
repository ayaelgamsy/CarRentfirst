using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerReportDto
{
    public class CustomerReportRegisterDto
    {
        public Guid? CustomerEvaluationId { get; set; }

        [Required(ErrorMessage = "*")]
        public IEnumerable<CustomerEvaluation> CustomerEvaluations { get; set; }
    }
}
