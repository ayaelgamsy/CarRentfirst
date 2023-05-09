using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarDto
{
    public class CarRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        public string Number { get; set; }

        [Required(ErrorMessage = "*")]
        public string CarModel { get; set; }

        [Required(ErrorMessage = "*")]
        public double? PriceOfDaye { get; set; }
        public double? MinPriceOfDaye { get; set; }

        public double? PricePerKm { get; set; }
        public double? PriceTimeLatePerHoure { get; set; }
        public double? PriceWash { get; set; }
        public double? PriceRentOwnerPerMonth { get; set; }
        public string GpsNumber { get; set; }
        public bool Active { get; set; }
        public bool Available { get; set; } = true;
        public DateTime? EndTripDate { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CompanyId { get; set; }
        public IEnumerable<Company> Companys { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarOwnertId { get; set; }
        public IEnumerable<CarOwner> CarOwners { get; set; }

        public List<CarPhoto> CarPhotos { get; set; }
        public List<CarVideo> CarVideos { get; set; }
        public List<IFormFile> CarPhotoFile { get; set; }
        public List<IFormFile> CarVideoFile { get; set; }


    }
}
