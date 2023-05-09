using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Dtos.CarStatusReportDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
    //[AllowAnonymous]
    public class CarStatusReportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<CustomerRent> _customerRentRepo;

        public CarStatusReportController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<Car> carRepo,
            IRepository<Customer> customerRepo,
            IRepository<CustomerRent> customerRentRepo
            )
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _carRepo = carRepo;
            _customerRepo = customerRepo;
            _customerRentRepo = customerRentRepo;
        }

        [Authorize("Permissions.CarStatusReportIndex")]
        public async Task<IActionResult> Index()
        {
            var customerRentData = await _customerRentRepo.GetAllAsync(c => c.Customer, c => c.Employee, c => c.Car);
            var carStatusGetDto = _mapper.Map<List<CarStatusGetDto>>(customerRentData);
            var carStatusRegisterDto = new CarStatusRegisterDto
            {
                Cars = await _carRepo.GetAllAsync()
            };
            var carStatusModelDto = new CarStatusModelDto
            {
                CarStatusGetDtos = carStatusGetDto,
                CarStatusRegisterDto = carStatusRegisterDto
            };
            return View(carStatusModelDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CarStatusReportCreate")]
        public async Task<IActionResult> Create(CarStatusModelDto model)
        {
            if (ModelState.IsValid)
            {
                //var car = _carRepo.SingleOrDefaultAsync(s => s.Id == model.CarStatusRegisterDto.CarId, s => s.CustomerRents).Result;
                //var customerRents = car.CustomerRents.Where(s => s.StartDate >= model.CarStatusRegisterDto.FromDate).Where(s => s.StartDate <= model.CarStatusRegisterDto.ToDate);
                var customerRentData =_customerRentRepo.GetAllAsync(c => c.CarId == model.CarStatusRegisterDto.CarId,c=>c.Car, c => c.Customer, c => c.Employee).Result
                                                        .Where(s => s.StartDate >= model.CarStatusRegisterDto.FromDate)
                                                        .Where(s => s.StartDate <= model.CarStatusRegisterDto.ToDate);
                var carStatusGetDto = _mapper.Map<List<CarStatusGetDto>>(customerRentData);
                var carStatusRegisterDto = new CarStatusRegisterDto
                {
                    Cars = await _carRepo.GetAllAsync()
                };
                var carStatusModelDto = new CarStatusModelDto
                {
                    CarStatusGetDtos= carStatusGetDto,
                    CarStatusRegisterDto= carStatusRegisterDto
                };
                return View("Index", carStatusModelDto);
            }
            return BadRequest();

        }
    }
}
