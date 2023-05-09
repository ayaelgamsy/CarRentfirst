using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerReservationDto
{
    public class CustomerReservationRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Customer { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarId { get; set; }
        public IEnumerable<Car> Cars { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid MarketerId { get; set; }
        public IEnumerable<Marketer> Marketers { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public DateTime? StartDate { get; set; }
        public double? NumberOfDays { get; set; }
        public DateTime? EndDate { get; set; }
        public double? AllowedDistance { get; set; }
        public double? Value { get; set; }
        public List<CustomerReservationPhoto> CustomerReservationPhotos { get; set; }
        public List<IFormFile> ReservationPhotoFile { get; set; }
        public string Notes { get; set; }
    }
}
