using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.ViolationReportDto
{
    public class ViolationReportModelDto
    {
        public List<ViolationReportGetDto> ViolationReportGetDtos { get; set; }
        public ViolationReportRegisterDto ViolationReportRegisterDto { get; set; }
    }
}
