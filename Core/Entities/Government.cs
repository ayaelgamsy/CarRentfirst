using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common;
namespace Core.Entities
{
    public class Government : EntityBaseName
    {

        public string Code { get; set; }

        public List<CarOwner> CarOwners { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Customer> Customers { get; set; }
        

    }
}
