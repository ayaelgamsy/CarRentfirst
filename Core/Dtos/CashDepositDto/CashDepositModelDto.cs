using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CashDepositDto
{
    public class CashDepositModelDto
    {
        public List<CashDepositGetDto> CashDepositGetDtos { get; set; }
        public CashDepositRegisterDto CashDepositRegisterDto { get; set; }
    }
}
