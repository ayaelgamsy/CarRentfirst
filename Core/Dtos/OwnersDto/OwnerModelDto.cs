using Core.Dtos.DropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnersDto
{
    public class OwnerModelDto
    {
        public OwnerModelDto()
        {
            OwnerRegister = new OwnerRegisterDto();
            OwnerRegister.Government = new List<GovernmentDrop>();
        }
        public OwnerRegisterDto OwnerRegister { get; set; }
        public List<OwnerGetDto> OwnerGetDtos { get; set; }
    }
}
