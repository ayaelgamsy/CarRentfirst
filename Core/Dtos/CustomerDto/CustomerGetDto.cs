using Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.CustomerDto
{
    public class CustomerGetDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string phone3 { get; set; }
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string GovernmentName { get; set; }
        public string Country { get; set; }
        public string CustomerEvaluationName { get; set; }
        public string GobTitle { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public double StartAccount { get; set; }
        public DateTime Date { get; set; }

        public List<CustomerPhoto> CustomerPhotos { get; set; }
        
    }
}
