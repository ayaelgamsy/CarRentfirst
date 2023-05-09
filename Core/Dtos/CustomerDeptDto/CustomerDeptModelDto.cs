using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDeptDto
{
    public class CustomerDeptModelDto
    {
        public CustomerDeptRegisterDto CustomerDeptRegisterDto { get; set; }
        public List<CustomerDeptGetDto> CustomerDeptGetDtos { get; set; }
    }
}
