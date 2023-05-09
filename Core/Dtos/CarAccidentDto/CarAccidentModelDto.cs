using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAccidentDto
{
    public class CarAccidentModelDto
    {
        public List<CarAccidentGetDto> CarAccidentGetDtos { get; set; }
        public CarAccidentRegisterDto CarAccidentRegisterDto { get; set; }
    }
}
