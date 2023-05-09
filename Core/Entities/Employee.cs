using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
  public  class Employee : EntityBaseName
    {
        public string Phone { get; set; }
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }


        [ForeignKey("Government")]
        public Guid? GovernmentId { get; set; }
        public Government Government { get; set; }
        public string Country { get; set; }

        public List<EmployeePhoto> EmployeePhotos { get; set; }
        public List<CustomerRent> CustomerRents { get; set; }
        public List<CustomerRent> CustomerRentReceives { get; set; }
       


    }
}
