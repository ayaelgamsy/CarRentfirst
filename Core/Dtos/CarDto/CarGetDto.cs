using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarDto
{
    public class CarGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
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
        public DateTime? EndTripDate { get; set; }
        public string CompanyName { get; set; }
        public string CarOwnerName { get; set; }

        public List<CarPhoto> CarPhotos { get; set; }
        public List<CarVideo> CarVideos { get; set; }
  
    }
}
