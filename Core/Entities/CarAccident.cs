using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CarAccident:EntityBase
    {
        public DateTime Date { get; set; }
        public double Price { get; set; }
        public string Notes { get; set; }
        public double Payment { get; set; }
        public double RestValue { get; set; }


        [ForeignKey("Car")]
        public Guid CarId { get; set; }
        public Car Car { get; set; }

        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }

        [ForeignKey("Stock")]
        public Guid StockId { get; set; }
        public Stock Stock { get; set; }

        public List<CarAccidentPhoto> CarAccidentPhotos { get; set; }
        public List<CarAccidentVideo> CarAccidentVideos { get; set; }






    }
}
