using AutoMapper;
using Core.Dtos.CustomerReportDto;
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
    public class CustomerReportController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<CustomerEvaluation> _customerEvaluationRepo;

        public CustomerReportController(IMapper mapper,
            IRepository<Customer> customerRepo,
            IRepository<CustomerEvaluation> customerEvaluationRepo)
        {
            _mapper = mapper;
            _customerRepo = customerRepo;
            _customerEvaluationRepo = customerEvaluationRepo;
        }

        [Authorize("Permissions.CustomerReportIndex")]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerRepo.GetAllAsync(c=>c.Government,c=>c.CustomerEvaluation);
            var customerReportGetDto = _mapper.Map<List<CustomerReportGetDto>>(customers);
            var customerReportRegisterDto = new CustomerReportRegisterDto
            {
                CustomerEvaluations = await _customerEvaluationRepo.GetAllAsync()
            };
            var customerReportModelDto = new CustomerReportModelDto
            {
                CustomerReportGetDtos = customerReportGetDto,
                CustomerReportRegisterDto = customerReportRegisterDto
            };
            return View(customerReportModelDto);
        }

        [Authorize("Permissions.CustomerReportCreate")]
        public async Task<IActionResult> Create(CustomerReportModelDto model)
        {
            var customers = _customerRepo.GetAllAsync(c => c.Government, c => c.CustomerEvaluation).Result
                .Where(c => c.CustomerEvaluationId == model.CustomerReportRegisterDto.CustomerEvaluationId);
            var customerReportGetDto = _mapper.Map<List<CustomerReportGetDto>>(customers);
            var customerReportRegisterDto = new CustomerReportRegisterDto
            {
                CustomerEvaluations = await _customerEvaluationRepo.GetAllAsync()
            };

            var customerReportModelDto = new CustomerReportModelDto
            {
                CustomerReportGetDtos = customerReportGetDto,
                CustomerReportRegisterDto = customerReportRegisterDto
            };
            return View("index",customerReportModelDto);
        }
    }
}
