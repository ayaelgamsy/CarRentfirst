using AutoMapper;
using Core.Common.enums;
using Core.Dtos.OwnerAccountDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Owners.Controllers
{
    [Area("Owners")]
    //[AllowAnonymous]
    public class OwnerAccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CarOwnerAccount> ownerAccountRepo;
        private readonly IRepository<CarOwner> carOwnerRepo;

        public OwnerAccountController(IMapper mapper,
            IRepository<CarOwnerAccount> OwnerAccountRepo,
            IRepository<CarOwner> CarOwnerRepo
            )
        {
            _mapper = mapper;
            ownerAccountRepo = OwnerAccountRepo;
            carOwnerRepo = CarOwnerRepo;
        }

        [Authorize("Permissions.OwnerAccountIndex")]
        public async Task<IActionResult> Index()
        {
            var ownerAccountData = await ownerAccountRepo.GetAllAsync();
            var ownerAccountGetDto = _mapper.Map<List<OwnerAccountGetDto>>(ownerAccountData);
            var ownerAccountRegisterDto = new OwnerAccountRegisterDto
            {
                CarOwners =await carOwnerRepo.GetAllAsync()
            };
            var ownerAccountModelDto = new OwnerAccountModelDto
            {
                OwnerAccountGetDtos = ownerAccountGetDto,
                OwnerAccountRegisterDto = ownerAccountRegisterDto
            };
            return View(ownerAccountModelDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerAccountCreate")]

        public async Task<IActionResult> Create(OwnerAccountModelDto model)
        {
            if (ModelState.IsValid)
            {
                var carOwner = carOwnerRepo.SingleOrDefaultAsync(c => c.Id == model.OwnerAccountRegisterDto.OwnerId, c => c.CarOwnerAccounts).Result;
                var OwnerAccounts = carOwner.CarOwnerAccounts.Where(o => o.Date >= model.OwnerAccountRegisterDto.FromDate).Where(o => o.Date <= model.OwnerAccountRegisterDto.ToDate);
                var ownerAccountGetDto = _mapper.Map<List<OwnerAccountGetDto>>(OwnerAccounts);

                var ownerAccountRegisterDto = new OwnerAccountRegisterDto
                {
                    CarOwners = await carOwnerRepo.GetAllAsync()
                };
                var ownerAccountModelDto = new OwnerAccountModelDto
                {
                    OwnerAccountGetDtos = ownerAccountGetDto,
                    OwnerAccountRegisterDto = ownerAccountRegisterDto
                };
                return View("Index", ownerAccountModelDto);
            }
            return BadRequest();  
        }

        public IActionResult GetPayment(Guid id, RentAccountType type)
        {
            if (type == RentAccountType.Rent)
            {
                 
                return RedirectToAction("Edit", "OwnerRent", new { area = "Owners", id = id });
            }
            else
            {
                return RedirectToAction("Edit", "OwnerPayment", new { area = "Owners", id = id });
            }
        }
    }
}
