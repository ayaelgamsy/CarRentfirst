using Core.Dtos.ExpenseDto;
using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAccidentDto
{
    public class CarAccidentRegisterDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "*")]
        public DateTime? Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "*")]
        public double? Price { get; set; }

        [Required(ErrorMessage = "*")]
        public double? Payment { get; set; }
        public double? RestValue { get; set; }
        public string Notes { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CarId { get; set; }
        public IEnumerable<DrpDto> Cars { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid CustomerId { get; set; }
        public IEnumerable<DrpDto> Customers { get; set; }

        [Required(ErrorMessage = "*")]
        public Guid StockId { get; set; }
        public IEnumerable<DrpDto> Stocks { get; set; }

        public List<CarAccidentPhoto> CarAccidentPhotos { get; set; }
        public List<CarAccidentVideo> CarAccidentVideos { get; set; }
        public List<IFormFile> CarAccidentPhotosFile { get; set; }
        public List<IFormFile> CarAccidentVideosFile { get; set; }

    }
}
