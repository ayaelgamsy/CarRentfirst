using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CarAccidentDto
{
    public class CarAccidentGetDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string Notes { get; set; }
        public double Payment { get; set; }
        public double RestValue { get; set; }
        public string CarName { get; set; }
        public string CustomerName { get; set; }
        public string StockName { get; set; }

        public List<CarAccidentPhoto> CarAccidentPhotos { get; set; }
        public List<CarAccidentVideo> CarAccidentVideos { get; set; }
    }
}
