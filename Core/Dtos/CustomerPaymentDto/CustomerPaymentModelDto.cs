using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerPaymentDto
{
    public class CustomerPaymentModelDto
    {
        public List<CustomerPayGetDto> CustomerPayGetDtos { get; set; }
        public CustomerPayRegisterDto CustomerPayRegisterDto { get; set; }
    }
}
