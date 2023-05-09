using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockTransferDto
{
   public class StockTransferModelDto
    {
        public List<StockTransferGetDto> StockTransferGetDtos { get; set; }
        public StockTransferRegisterDto StockTransferRegisterDto { get; set; }
    }
}
