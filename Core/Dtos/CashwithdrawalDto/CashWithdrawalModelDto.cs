using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CashwithdrawalDto
{
    public class CashWithdrawalModelDto
    {
        public List<CashwithdrawalGetDto> CashwithdrawalGetDtos { get; set; }
        public CashwithdrawalRegisterDto CashwithdrawalRegisterDto { get; set; }
    }
}
