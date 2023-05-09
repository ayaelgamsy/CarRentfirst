using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerEvaluationDto
{
    public class CustomerEvaluationModelDto
    {
        public List<CustomerEvaluationGetDto> CustomerEvaluationGetDtos { get; set; }
        public CustomerEvaluationRegisterDto CustomerEvaluationRegisterDto { get; set; }
    }
}
