using Core.Dtos.DropDowns;
using Core.Entities;
using System;
using System.Collections.Generic;

namespace Core.Dtos.OwnersDto
{
    public class OwnerGetDto
    {
        public Guid? Id { get; set; }
        public string name { get; set; }
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string GovernmentName { get; set; }
        public string Country { get; set; }
        public List<CarOwnerPhoto> CarOwnerPhotos { get; set; } = new List<CarOwnerPhoto>();
    }
}
