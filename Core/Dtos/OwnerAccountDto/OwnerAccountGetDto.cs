using Core.Common.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.OwnerAccountDto
{
    public class OwnerAccountGetDto
    {
        public Guid Id { get; set; }

        public Guid MovementId { get; set; }
        public RentAccountType AccountType { get; set; }
        public double Dept { get; set; }
        public double Borrower { get; set; }
        public double Account { get; set; }
        public string Notes { get; set; }
        public string Explain { get; set; }

    }
}
