using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDto
{
    public class CustomerDataDto
    {
        public string Name { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string CreatedUser { get; set; }
        public string LastEditUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastEditDate { get; set; }

    }
}
