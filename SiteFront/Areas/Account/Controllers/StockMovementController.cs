using AutoMapper;
using Core.Common.enums;
using Core.Dtos.StockMovementDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Account.Controllers
{
    [Area("Account")]
    //[AllowAnonymous]
    public class StockMovementController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<StockMovement> stockMovementRepo;
        private readonly IRepository<Stock> stockRepo;

        public StockMovementController(IMapper mapper,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<Stock> StockRepo)
        {
            _mapper = mapper;
            stockMovementRepo = StockMovementRepo;
            stockRepo = StockRepo;
        }

        [Authorize("Permissions.StockMovementIndex")]
        public async Task<IActionResult> Index()
        {
            var StockMovementData =await stockMovementRepo.GetAllAsync();
            var stockMovementGetDto = _mapper.Map<List<StockMovementGetDto>>(StockMovementData);
            var stockMovementRegisterDto = new StockMovementRegisterDto
            {
                Stocks = await stockRepo.GetAllAsync()
            };
            var stockMovementModelDto = new StockMovementModelDto
            {
                StockMovementGetDtos = stockMovementGetDto,
                StockMovementRegisterDto = stockMovementRegisterDto
            };
            return View(stockMovementModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.StockMovementCreate")]

        public async Task<IActionResult> Create(StockMovementModelDto model)
        {
            if (ModelState.IsValid) { 
            var stock = stockRepo.SingleOrDefaultAsync(s => s.Id == model.StockMovementRegisterDto.StockId,s=>s.StockMovement).Result;
            var stockMovement = stock.StockMovement.Where(s => s.Date.Date >= model.StockMovementRegisterDto.FromDate.Value.Date && s.Date <= model.StockMovementRegisterDto.ToDate.Value.Date);
            var stockMovementGetDto = _mapper.Map<List<StockMovementGetDto>>(stockMovement);
            var stockMovementRegisterDto = new StockMovementRegisterDto
            {
                Stocks = await stockRepo.GetAllAsync()
            };
            var stockMovementModelDto = new StockMovementModelDto
            {
                StockMovementGetDtos = stockMovementGetDto,
                StockMovementRegisterDto = stockMovementRegisterDto
            };
            return View("Index", stockMovementModelDto);
            }
            return BadRequest();

        }

        public IActionResult GetMovement(Guid id, StockMovementType type)
        {        
            if(type == StockMovementType.OwnerRent)
            {
                return RedirectToAction("Edit", "OwnerRent", new { area = "Owners", id = id });
            }
            else if(type == StockMovementType.OwnerPayment)
            {
                return RedirectToAction("Edit", "OwnerPayment", new { area = "Owners",id= id });
            }
            else if (type == StockMovementType.Accident)
            {
                return RedirectToAction("Edit", "CarAccident", new { area = "Owners", id = id });
            }
            else if (type == StockMovementType.CarMaintenance)
            {
                return RedirectToAction("Edit", "CarMaintenance", new { area = "Owners", id = id });
            }
            else if (type == StockMovementType.CustomerRent)
            {
                return RedirectToAction("Edit", "CustomerRent", new { area = "Rent", id = id });
            }
            else if (type == StockMovementType.CustomerPayment)
            {
                return RedirectToAction("Edit", "CustomerPayment", new { area = "Rent", id = id });
            }
            else if (type == StockMovementType.CustomerViolation)
            {
                return RedirectToAction("Edit", "CustomerViolation", new { area = "Rent", id = id });
            }
            else if (type == StockMovementType.CustomerRentBack)
            {
                return RedirectToAction("EditRentBack", "CareStats", new { area = "Rent", id = id });
            }
            else if (type == StockMovementType.CashDeposit)
            {
                return RedirectToAction("Edit", "CashDeposit", new { area = "Account", id = id });
            }
            else if (type == StockMovementType.Cashwithdrawal)
            {
                return RedirectToAction("Edit", "Cashwithdrawal", new { area = "Account", id = id });
            }
            else if (type == StockMovementType.Expense)
            {
                return RedirectToAction("Create", "Expenses", new { area = "Account", id = id });
            }
            else if (type == StockMovementType.StockTransferto)
            {
                return RedirectToAction("Edit", "StockTransfer", new { area = "Account", id = id });
            }
            else if (type == StockMovementType.StockTransferfrom)
            {
                return RedirectToAction("Edit", "StockTransfer", new { area = "Account", id = id });
            }
            else 
            {
                return RedirectToAction("Index", "Stock", new { area = "Account", id = id });
            }

        }
    }
}
