using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAvailableByDateDto
{
    public class CarAvailableByDateModelDto
    {
        public List<CarAvailableByDateGetDto> CarAvailableByDateGetDtos { get; set; }
        public CarAvailableByDateRegisterDto CarAvailableByDateRegisterDto { get; set; }
    }
}
