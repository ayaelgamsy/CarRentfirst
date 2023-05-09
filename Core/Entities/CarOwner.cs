using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public class CarOwner : EntityBaseName
    {

       public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        [ForeignKey("Government")]
        public Guid? GovernmentId { get; set; }
        public Government Government { get; set; }
        public string Country { get; set; }

        public double StartAccount { get; set; }
        public DateTime Date { get; set; }


        public List<CarOwnerPhoto> CarOwnerPhotos { get; set; }

        public List<Car> Cars { get; set; }
        public List<OwnerRentContract> OwnerRentContracts { get; set; }
        public List<CarOwnerAccount> CarOwnerAccounts { get; set; }
    }
}
