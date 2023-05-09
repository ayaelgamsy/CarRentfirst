using AutoMapper;
using Core.Dtos.ViolationReportDto;
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
    public class ViolationReportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CustomerViolation> _customerViolationRepo;
        private readonly IRepository<Car> _carRepo;

        public ViolationReportController(IMapper mapper,
            IRepository<CustomerViolation> customerViolationRepo,
            IRepository<Car> carRepo)
        {
            _mapper = mapper;
            _customerViolationRepo = customerViolationRepo;
            _carRepo = carRepo;
        }

       [Authorize("Permissions.ViolationReportIndex")]
        public async Task<IActionResult> Index()
        {
            var customerViolationData = await _customerViolationRepo.GetAllAsync(c => c.Customer, c => c.Car);
            var violationReportGetDto = _mapper.Map<List<ViolationReportGetDto>>(customerViolationData);
            var violationReportRegisterDto = new ViolationReportRegisterDto
            {
                Cars = await _carRepo.GetAllAsync()
            };
            var violationReportModelDto = new ViolationReportModelDto
            {
              ViolationReportGetDtos= violationReportGetDto,
              ViolationReportRegisterDto= violationReportRegisterDto
            };
            return View(violationReportModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.ViolationReportCreate")]
        public async Task<IActionResult> Create(ViolationReportModelDto model)
        {
            if (ModelState.IsValid)
            {
                var customerViolationData =await _customerViolationRepo.GetAllAsync(c => c.CarId == model.ViolationReportRegisterDto.CarId, c => c.Car, c => c.Customer);
                                                 //.Where(c => c.CarId== model.ViolationReportRegisterDto.CarId);

                var violationReportGetDto = _mapper.Map<List<ViolationReportGetDto>>(customerViolationData);
                var violationReportRegisterDto = new ViolationReportRegisterDto
                {
                    Cars = await _carRepo.GetAllAsync()
                };
                var violationReportModelDto = new ViolationReportModelDto
                {
                    ViolationReportGetDtos = violationReportGetDto,
                    ViolationReportRegisterDto = violationReportRegisterDto
                };
                return View("Index", violationReportModelDto);
            }
            return BadRequest();

        }
    }
}
