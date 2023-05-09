using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDto
{
    public class CustomerModelDto
    {
        public List<CustomerGetDto> CustomerGetDtos { get; set; }
        public CustomerRegisterDto CustomerRegisterDto { get; set; }
    }
}
