using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
   public class Car : EntityBaseName
    {
        public string Number { get; set; }
        public string CarModel { get; set; }
        public double PriceOfDaye { get; set; }
        public double MinPriceOfDaye { get; set; }

        public double PricePerKm { get; set; }
        public double PriceTimeLatePerHoure { get; set; }
        public double PriceWash { get; set; }
        public double PriceRentOwnerPerMonth { get; set; }
        public string GpsNumber { get; set; }
        public bool Active { get; set; }
        public bool Available { get; set; }
        public DateTime EndTripDate { get; set; }

        [ForeignKey("Company")]
        public Guid CompanyId { get; set; }
        public  Company Company { get; set; }

        [ForeignKey("CarOwner")]
        public Guid CarOwnertId { get; set; }
        public CarOwner CarOwner { get; set; }

        public List<OwnerRentContract> OwnerRentContracts { get; set; }
        public List<CustomerRent> CustomerRents { get; set; }
        public List<CarPhoto> CarPhotos { get; set; }
        public List<CarVideo> CarVideos { get; set; }
        public List<CarMaintenance> carMaintenances { get; set; }


    }
}
