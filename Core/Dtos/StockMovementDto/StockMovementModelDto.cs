using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockMovementDto
{
    public class StockMovementModelDto
    {
        public StockMovementRegisterDto StockMovementRegisterDto { get; set; }
        public List<StockMovementGetDto> StockMovementGetDtos { get; set; }
    }
}
