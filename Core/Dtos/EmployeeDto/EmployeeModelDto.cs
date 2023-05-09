using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.EmployeeDto
{
    public class EmployeeModelDto
    {
        public EmployeeRegisterDto EmployeeRegisterDto { get; set; }
        public List<EmployeeGetDto> EmployeeGetDtos { get; set; }
    }
}
