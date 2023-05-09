using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerAccountDto
{
    public class OwnerAccountModelDto
    {
        public OwnerAccountRegisterDto OwnerAccountRegisterDto { get; set; }
        public List<OwnerAccountGetDto> OwnerAccountGetDtos { get; set; }
    }
}
