using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ExpenseDto
{
    public class ExpenseModelDto
    {
        public List<ExpenseGetDto> ExpenseGetDtos { get; set; }
        public ExpenseReportDto ExpenseReportDto { get; set; }
    }
}
