using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarMaintenanceDto
{
    public class CarMaintenanceModelDto
    {
        public List<CarMaintenanceGetDto> CarMaintenanceGetDtos { get; set; }
        public CarMaintenanceRegisterDto CarMaintenanceRegisterDto { get; set; }
    }
}
