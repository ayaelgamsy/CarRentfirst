using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.EmployeeDto
{
    public class EmployeeGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public string GovernmentName { get; set; }
        public string Country { get; set; }
        public List<EmployeePhoto> EmployeePhotos { get; set; }

    }
}
