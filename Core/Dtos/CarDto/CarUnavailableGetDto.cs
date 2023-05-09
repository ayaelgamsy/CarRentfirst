using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarDto
{
   public class CarUnavailableGetDto
    {
        public Guid CarId { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string CarModel { get; set; }
        public double PriceOfDaye { get; set; }
        // public bool Active { get; set; }
        // public bool Available { get; set; }
        public DateTime EndDate { get; set; }

        public string CompanyName { get; set; }


        public List<CarPhoto> CarPhotos { get; set; }

        public Guid CustomerRentId { get; set; }
    }
}
