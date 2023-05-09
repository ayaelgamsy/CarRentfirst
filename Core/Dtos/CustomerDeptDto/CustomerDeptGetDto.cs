using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDeptDto
{
    public class CustomerDeptGetDto
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public double DebtValue { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
    }
}
