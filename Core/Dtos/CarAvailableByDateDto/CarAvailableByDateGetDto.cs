using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAvailableByDateDto
{
    public class CarAvailableByDateGetDto
    {
        public Guid Id { get; set; }
        public string CarName { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone1 { get; set; }
        public string EmployeeName { get; set; }
        public DateTime EndDate { get; set; }
    }
}
