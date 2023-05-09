using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerViolationDto
{
    public class CustomerViolationRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage ="*")]
        public Guid CarId { get; set; }
        public IEnumerable<Car> Cars { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CustomerId { get; set; }
        public IEnumerable<Customer> Customers { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? ViolationDate { get; set; }

        [Required(ErrorMessage = "*")]
        public double? Value { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public double Payment { get; set; }

        [Required(ErrorMessage = "*")]
        public double? Rest { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<Stock>  Stocks { get; set; }

        [Required(ErrorMessage = "*")]
        public string ViolationNumber { get; set; }
        public string ViolationState { get; set; }
        public string ViolationPlace { get; set; }
        public List<CustomerViolationPhoto> CustomerViolationPhotos { get; set; }
        public List<IFormFile>  ViolationPhotoFile { get; set; }
        public string Notes { get; set; }

    }
}
