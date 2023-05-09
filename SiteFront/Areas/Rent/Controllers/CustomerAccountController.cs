using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerAccountDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
    //[AllowAnonymous]
    public class CustomerAccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CustomerAccount> customerAccountRepo;
        private readonly IRepository<Customer> customerRepo;

        public CustomerAccountController(IMapper mapper,
            IRepository<CustomerAccount> CustomerAccountRepo,
            IRepository<Customer> CustomerRepo
            )
        {
            _mapper = mapper;
            customerAccountRepo = CustomerAccountRepo;
            customerRepo = CustomerRepo;
        }

        [Authorize("Permissions.CustomerAccountIndex")]

        public async Task<IActionResult> Index()
        {
            var customerAccountData = await customerAccountRepo.GetAllAsync();
            var customerAccountGetDto = _mapper.Map<List<CustomerAccountGetDto>>(customerAccountData);
            var customerAccountRegisterDto = new CustomerAccountRegisterDto
            {
                Customers = await customerRepo.GetAllAsync()
            };
            var customerAccountModelDto = new CustomerAccountModelDto
            {
                CustomerAccountGetDtos = customerAccountGetDto,
                CustomerAccountRegisterDto = customerAccountRegisterDto
            };
            return View(customerAccountModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerAccountCreate")]

        public async Task<IActionResult> Create(CustomerAccountModelDto model)
        {
            if (ModelState.IsValid)
            {
                var customer = customerRepo.SingleOrDefaultAsync(c => c.Id == model.CustomerAccountRegisterDto.CustomerId, c => c.customerAccounts).Result;
                var CustomerAccounts = customer.customerAccounts.Where(o => o.Date >= model.CustomerAccountRegisterDto.FromDate).Where(o => o.Date <= model.CustomerAccountRegisterDto.ToDate);
                var customerAccountGetDto = _mapper.Map<List<CustomerAccountGetDto>>(CustomerAccounts);

                var customerAccountRegisterDto = new CustomerAccountRegisterDto
                {
                    Customers = await customerRepo.GetAllAsync()
                };
                var customerAccountModelDto = new CustomerAccountModelDto
                {
                    CustomerAccountGetDtos = customerAccountGetDto,
                    CustomerAccountRegisterDto = customerAccountRegisterDto
                };
                return View("Index", customerAccountModelDto);
            }
            return BadRequest();

        }

        public IActionResult GetPayment(Guid id, RentAccountType type)
        {
            if (type == RentAccountType.Rent)
            {
                return RedirectToAction("Edit", "CustomerRent", new { area = "Rent", id = id });
            }
            else if (type == RentAccountType.payment)
            {
                return RedirectToAction("Edit", "CustomerPayment", new { area = "Rent", id = id });
            }
            else if (type == RentAccountType.RentBack)
            {
                return RedirectToAction("EditRentBack", "CareStats", new { area = "Rent", id = id });
            }
            else if (type == RentAccountType.Violation)
            {
                return RedirectToAction("Edit", "CustomerViolation", new { area = "Rent", id = id });
            }
            else if (type == RentAccountType.Accident)
            {
                return RedirectToAction("Edit", "CarAccident", new { area = "Owners", id = id });
            }
            else if (type == RentAccountType.LastDept)
            {
                return RedirectToAction("Edit", "CustomerDept", new { area = "Rent", id = id });
            }
            else 
            {
                return RedirectToAction("Edit", "Customers", new { area = "Rent", id = id });
            }
        }
    }
}
