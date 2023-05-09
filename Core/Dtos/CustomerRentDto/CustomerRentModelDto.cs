using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerRentDto
{
    public class CustomerRentModelDto
    {
        public List<CustomerRentGetDto> CustomerRentGetDtos { get; set; }
        public CustomerRentRegisterDto CustomerRentRegisterDto { get; set; }
    }
}
