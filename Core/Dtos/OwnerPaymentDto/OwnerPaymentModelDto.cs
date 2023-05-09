using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerPaymentDto
{
   public class OwnerPaymentModelDto
    {
        public List<OwnerPaymentGetDto> OwnerPaymentGetDtos { get; set; }
        public OwnerPaymentRegisterDto OwnerPaymentRegisterDto { get; set; }
    }
}
