using Core.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
   public class Customer : EntityBaseName
    {

        public string Address { get; set; }
        public string Email { get; set; }
        public string phone1 { get; set; }
        public string phone2 { get; set; }
        public string phone3 { get; set; }       
        public string IdentNumber { get; set; }
        public string PassportNumber { get; set; }
        public string GobTitle { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public double StartAccount { get; set; }
        public DateTime Date { get; set; }


        [ForeignKey("CustomerEvaluation")]
        public Guid? CustomerEvaluationId { get; set; }
        public CustomerEvaluation CustomerEvaluation { get; set; }


        [ForeignKey("Government")]
        public Guid? GovernmentId { get; set; }
        public Government Government { get; set; }

        public string Country { get; set; }

        public List<CustomerPhoto> CustomerPhotos { get; set; }
        public List<CustomerPayment> customerPayments  { get; set; }
        public List<CustomerRent> CustomerRents  { get; set; }
        public List<CustomerAccount> customerAccounts { get; set; }
        public List<CustomerLastDept> CustomerLastDepts { get; set; }
        
    }
}
