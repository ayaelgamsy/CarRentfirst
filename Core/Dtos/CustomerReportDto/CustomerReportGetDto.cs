using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerReportDto
{
    public class CustomerReportGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string phone1 { get; set; }
        public string IdentNumber { get; set; }
        public string GovernmentName { get; set; }
        public string PassportNumber { get; set; }

        public string Country { get; set; }
        public string GobTitle { get; set; }



        public string CustomerEvaluationName { get; set; }


       

        
    }
}
