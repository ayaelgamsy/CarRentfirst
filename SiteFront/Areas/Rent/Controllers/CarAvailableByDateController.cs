using AutoMapper;
using Core.Dtos.CarAvailableByDateDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
    //[AllowAnonymous]
    public class CarAvailableByDateController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CustomerRent> _customerRentRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Customer> _customerRepo;

        public CarAvailableByDateController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CustomerRent> customerRentRepo,
            IRepository<Car> carRepo,
            IRepository<Customer> customerRepo)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _customerRentRepo = customerRentRepo;
            _carRepo = carRepo;
            _customerRepo = customerRepo;
        }

        [Authorize("Permissions.CarAvailableByDateIndex")]
        public async Task<IActionResult> Index()
        {
            var customerRentData = await _customerRentRepo.GetAllAsync(n => n.Finished == false, c => c.Customer, c => c.Employee, c => c.Car);
            var carAvailableByDateGetDto = _mapper.Map<List<CarAvailableByDateGetDto>>(customerRentData);
            var carAvailableByDateRegisterDto = new CarAvailableByDateRegisterDto();
            var carAvailableByDateModelDto = new CarAvailableByDateModelDto
            {
                CarAvailableByDateGetDtos= carAvailableByDateGetDto,
                CarAvailableByDateRegisterDto= carAvailableByDateRegisterDto
            };
            return View(carAvailableByDateModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CarAvailableByDateCreate")]
        public async Task<IActionResult> Create(CarAvailableByDateModelDto model)
        {
            if (ModelState.IsValid)
            {
                var customerRentData = _customerRentRepo.GetAllAsync(n => n.Finished == false, c => c.Car, c => c.Customer, c => c.Employee).Result
                                                 .Where(c => c.EndDate.Date.CompareTo(model.CarAvailableByDateRegisterDto.EndDate.Value.Date) == 0);
                                       
                var carAvailableByDateGetDto = _mapper.Map<List<CarAvailableByDateGetDto>>(customerRentData);
                var carAvailableByDateRegisterDto = new CarAvailableByDateRegisterDto();
                var carAvailableByDateModelDto = new CarAvailableByDateModelDto
                {
                    CarAvailableByDateGetDtos = carAvailableByDateGetDto,
                    CarAvailableByDateRegisterDto = carAvailableByDateRegisterDto
                };
                return View("Index", carAvailableByDateModelDto);
            }
            return BadRequest();

        }
    }
}
