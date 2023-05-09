using Core.Common.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.StockMovementDto
{
    public class StockMovementGetDto
    {
        public Guid Id { get; set; }
        public Guid MovementId { get; set; }
        public StockMovementType MovementType { get; set; }
        public double InValue { get; set; }
        public double OutValue { get; set; }
        public double DifferentValue { get; set; }
        public double AccountValue { get; set; }
        public string Notes { get; set; }
        public string Comment { get; set; }
    }
}
