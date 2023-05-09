using AutoMapper;
using Core.Common.enums;
using Core.Dtos.ExpenseDto;
using Core.Dtos.OwnerPaymentDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Owners.Controllers
{
    [Area("Owners")]
    //[AllowAnonymous]
    public class OwnerPaymentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CarOwnerPayment> ownerPaymentRepo;
        private readonly IRepository<OwnerRentContract> ownerRentRepo;
        private readonly IRepository<Stock> stockRepo;
        private readonly IRepository<StockMovement> stockMovementRepo;
        private readonly IRepository<CarOwner> carOwnerRepo;
        private readonly IRepository<CarOwnerAccount> carOwnerAccountRepo;
        private readonly UserManager<User> _userManager;

        public OwnerPaymentController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CarOwnerPayment> OwnerPaymentRepo,
            IRepository<OwnerRentContract> OwnerRentRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<CarOwner> CarOwnerRepo,
            IRepository<CarOwnerAccount> CarOwnerAccountRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            ownerPaymentRepo = OwnerPaymentRepo;
            ownerRentRepo = OwnerRentRepo;
            stockRepo = StockRepo;
            stockMovementRepo = StockMovementRepo;
            carOwnerRepo = CarOwnerRepo;
            carOwnerAccountRepo = CarOwnerAccountRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.OwnerPaymentIndex")]
        public async Task<IActionResult> Index()
        {
            var OwnerPaymentData = await ownerPaymentRepo.GetAllAsync(c => c.CarOwner, c => c.Stock);
            var ownerPaymentGetDto = _mapper.Map<List<OwnerPaymentGetDto>>(OwnerPaymentData);
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerPaymentRegisterDto = new OwnerPaymentRegisterDto
            {
                CarOwners = _mapper.Map<List<DrpDto>>(CarOwners),
                Stocks = _mapper.Map<List<DrpDto>>(Stocks)
            };
            var ownerPaymentModelDto = new OwnerPaymentModelDto
            {
                OwnerPaymentGetDtos = ownerPaymentGetDto,
                OwnerPaymentRegisterDto = ownerPaymentRegisterDto
            };
            return View(ownerPaymentModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerPaymentCreate")]
        public async Task<IActionResult> Create(OwnerPaymentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var OwnerPaymentDb = _mapper.Map<CarOwnerPayment>(model);
                OwnerPaymentDb.CreatedDate = DateTime.Now;
                OwnerPaymentDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                ownerPaymentRepo.Add(OwnerPaymentDb);
                var carOwner = carOwnerRepo.SingleOrDefaultAsync(c => c.Id == OwnerPaymentDb.CarOwnerId).Result.name;

                StockMovement stockMovement = new StockMovement
                {
                    MovementId = OwnerPaymentDb.Id,
                    MovementType = StockMovementType.OwnerPayment,
                    StockId = OwnerPaymentDb.StockId,
                    Date = OwnerPaymentDb.Date,
                    OutValue = OwnerPaymentDb.Value,
                    InValue = 0,
                    Comment = " دفعة الي المالك"+" " + carOwner,
                    Notes = OwnerPaymentDb.Notes
                };
                stockMovementRepo.Add(stockMovement);

                CarOwnerAccount carOwnerAccount = new CarOwnerAccount
                {
                    MovementId = OwnerPaymentDb.Id,
                    AccountType = RentAccountType.payment,
                    Borrower = OwnerPaymentDb.Value,
                    Date = OwnerPaymentDb.Date,
                    Notes = OwnerPaymentDb.Notes,
                    CarOwnerId = OwnerPaymentDb.CarOwnerId,
                    Explain = " دفعة الي المالك"+" " + carOwner
                };
                carOwnerAccountRepo.Add(carOwnerAccount);

                await ownerRentRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var OwnerPaymentData = await ownerPaymentRepo.GetAllAsync(c => c.CarOwner, c => c.Stock);
                var ownerPaymentGetDto = _mapper.Map<List<OwnerPaymentGetDto>>(OwnerPaymentData);
                var CarOwners = await carOwnerRepo.GetAllAsync();
                var Stocks = await stockRepo.GetAllAsync();
                var ownerPaymentRegisterDto = _mapper.Map<OwnerPaymentRegisterDto>(model);
                ownerPaymentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
                ownerPaymentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var ownerPaymentModelDto = new OwnerPaymentModelDto
                {
                    OwnerPaymentGetDtos = ownerPaymentGetDto,
                    OwnerPaymentRegisterDto = ownerPaymentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index",ownerPaymentModelDto);
            }
        }

        [Authorize("Permissions.OwnerPaymentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var OwnerPaymentById = await ownerPaymentRepo.GetByIdAsync(id);
            if (OwnerPaymentById == null)
                return NotFound();
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerPaymentRegisterDto = _mapper.Map<OwnerPaymentRegisterDto>(OwnerPaymentById);
            ownerPaymentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
            ownerPaymentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialOwnerPayment", ownerPaymentRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.OwnerPaymentEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var OwnerPaymentById = await ownerPaymentRepo.GetByIdAsync(id);
            if (OwnerPaymentById == null)
                return NotFound();
            var OwnerPaymentData = await ownerPaymentRepo.GetAllAsync(c => c.CarOwner, c => c.Stock);
            var ownerPaymentGetDto = _mapper.Map<List<OwnerPaymentGetDto>>(OwnerPaymentData);
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerPaymentRegisterDto = _mapper.Map<OwnerPaymentRegisterDto>(OwnerPaymentById);
            ownerPaymentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
            ownerPaymentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            var ownerPaymentModelDto = new OwnerPaymentModelDto
            {
                OwnerPaymentGetDtos = ownerPaymentGetDto,
                OwnerPaymentRegisterDto = ownerPaymentRegisterDto
            };
            return View("Index", ownerPaymentModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerPaymentEdit")]

        public async Task<IActionResult> Edit(OwnerPaymentRegisterDto model)
        {
            if (ModelState.IsValid)
            { 
            var OwnerPaymentById = await ownerPaymentRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = OwnerPaymentById.CreatedUser;
                var userAddDate = OwnerPaymentById.CreatedDate;
                var OwnerPaymentDb = _mapper.Map(model, OwnerPaymentById);
                OwnerPaymentDb.CreatedDate = userAddDate;
                OwnerPaymentDb.CreatedUser = userAdd;
                OwnerPaymentDb.LastEditDate = DateTime.Now;
                OwnerPaymentDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                ownerPaymentRepo.Update(OwnerPaymentDb);
            var carOwner = carOwnerRepo.SingleOrDefaultAsync(c => c.Id == OwnerPaymentDb.CarOwnerId).Result.name;

                var StockMovementById = await stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == model.Id && s.MovementType == StockMovementType.OwnerPayment);
           
            StockMovementById.StockId = OwnerPaymentDb.StockId;
            StockMovementById.Date = (DateTime)OwnerPaymentDb.Date;
            StockMovementById.OutValue = (double)OwnerPaymentDb.Value;
            StockMovementById.InValue = 0;
            StockMovementById.Comment = " دفعة الي المالك "+" " + carOwner;
            StockMovementById.Notes = OwnerPaymentDb.Notes;

            stockMovementRepo.Update(StockMovementById);

                var CarOwnerAccountById = await carOwnerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == model.Id && c.AccountType == RentAccountType.payment);
           
            CarOwnerAccountById.Borrower = (double)OwnerPaymentDb.Value;
            CarOwnerAccountById.Date = (DateTime)OwnerPaymentDb.Date;
            CarOwnerAccountById.Notes = OwnerPaymentDb.Notes;
            CarOwnerAccountById.CarOwnerId = OwnerPaymentDb.CarOwnerId;
            CarOwnerAccountById.Explain = "  دفعة الي المالك" +" "+ carOwner;

            carOwnerAccountRepo.Update(CarOwnerAccountById);

            await ownerRentRepo.SaveAllAsync();
            _toastNotification.AddSuccessToastMessage("تم التعديل");
            return RedirectToAction("Index");
            }
            else
            {
                var OwnerPaymentData = await ownerPaymentRepo.GetAllAsync(c => c.CarOwner, c => c.Stock);
                var ownerPaymentGetDto = _mapper.Map<List<OwnerPaymentGetDto>>(OwnerPaymentData);
                var CarOwners = await carOwnerRepo.GetAllAsync();
                var Stocks = await stockRepo.GetAllAsync();
                var ownerPaymentRegisterDto = _mapper.Map<OwnerPaymentRegisterDto>(model);
                ownerPaymentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
                ownerPaymentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var ownerPaymentModelDto = new OwnerPaymentModelDto
                {
                    OwnerPaymentGetDtos = ownerPaymentGetDto,
                    OwnerPaymentRegisterDto = ownerPaymentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", ownerPaymentModelDto);
            }
        }

        [Authorize("Permissions.OwnerPaymentDelete")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var OwnerPaymentById = await ownerPaymentRepo.GetByIdAsync(id);
            if (OwnerPaymentById == null)
                return NotFound();
            var CarOwnerAccountById = await carOwnerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == id && c.AccountType == RentAccountType.payment);
            var StockMovementById = await stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == id && s.MovementType == StockMovementType.OwnerPayment);
            carOwnerAccountRepo.Delete(CarOwnerAccountById);
            stockMovementRepo.Delete(StockMovementById);
            ownerPaymentRepo.Delete(OwnerPaymentById);
            await ownerPaymentRepo.SaveAllAsync();
            return Ok();

        }

        public async Task<IActionResult> GetCurrentDebt(Guid id)
        {
            var OwnerAccounts = await carOwnerRepo.SingleOrDefaultAsync(c => c.Id == id, c => c.CarOwnerAccounts);
            if (OwnerAccounts == null)
                return NotFound();

            return Json(new { currentDebt = OwnerAccounts.CarOwnerAccounts.Sum(c => c.Dept) - OwnerAccounts.CarOwnerAccounts.Sum(c => c.Borrower) });

        }

    }
}