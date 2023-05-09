using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerAccountDto
{
    public class CustomerAccountModelDto
    {
        public CustomerAccountRegisterDto CustomerAccountRegisterDto { get; set; }
        public List<CustomerAccountGetDto> CustomerAccountGetDtos { get; set; }
    }
}
