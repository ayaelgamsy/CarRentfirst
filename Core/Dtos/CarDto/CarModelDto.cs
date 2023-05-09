using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarDto
{
    public class CarModelDto
    {
        public CarRegisterDto CarRegisterDto { get; set; }
        public List<CarGetDto> CarGetDtos { get; set; }
    }
}
