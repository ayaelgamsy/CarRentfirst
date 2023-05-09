using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerRentDto
{
    public class OwnerRentModelDto
    {
        public List<OwnerRentGetDto> OwnerRentGetDtos { get; set; }
        public OwnerRentRegisterDto OwnerRentRegisterDto { get; set; }
    }
}
