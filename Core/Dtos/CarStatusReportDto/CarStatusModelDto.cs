using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarStatusReportDto
{
    public class CarStatusModelDto
    {
        public CarStatusRegisterDto CarStatusRegisterDto { get; set; }
        public List<CarStatusGetDto> CarStatusGetDtos { get; set; }
    }
}
