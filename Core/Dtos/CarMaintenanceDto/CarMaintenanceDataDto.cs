using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarMaintenanceDto
{
    public class CarMaintenanceDataDto
    {
        public string CarName { get; set; }

        public DateTime date { get; set; }
        public string Notes { get; set; }
        public double value { get; set; }
        public string StockName { get; set; }

        public string CreatedUser { get; set; }
        public string LastEditUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastEditDate { get; set; }
    }
}
