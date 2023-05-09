using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.MarketerDto
{
   public class MarketerModelDto
    {
        public MarketerRegisterDto MarketerRegisterDto { get; set; }
        public List<MarketerGetDto> MarketerGetDtos { get; set; }
    }
}
