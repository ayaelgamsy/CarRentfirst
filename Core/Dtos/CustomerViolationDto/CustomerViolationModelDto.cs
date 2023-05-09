using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerViolationDto
{
    public class CustomerViolationModelDto
    {
        public List<CustomerViolationGetDto> CustomerViolationGetDtos { get; set; }
        public CustomerViolationRegisterDto CustomerViolationRegisterDto { get; set; }
    }
}
