using AutoMapper;
using Core.Common.enums;
using Core.Dtos.ExpenseDto;
using Core.Dtos.OwnerRentDto;
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
    // [AllowAnonymous]
    public class OwnerRentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<OwnerRentContract> ownerRentRepo;
        private readonly IRepository<Stock> stockRepo;
        private readonly IRepository<StockMovement> stockMovementRepo;
        private readonly IRepository<CarOwner> carOwnerRepo;
        private readonly IRepository<Car> carRepo;
        private readonly IRepository<CarOwnerAccount> carOwnerAccountRepo;
        private readonly UserManager<User> _userManager;

        public OwnerRentController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<OwnerRentContract> OwnerRentRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<CarOwner> CarOwnerRepo,
            IRepository<Car> CarRepo,
            IRepository<CarOwnerAccount> CarOwnerAccountRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            ownerRentRepo = OwnerRentRepo;
            stockRepo = StockRepo;
            stockMovementRepo = StockMovementRepo;
            carOwnerRepo = CarOwnerRepo;
            carRepo = CarRepo;
            carOwnerAccountRepo = CarOwnerAccountRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.OwnerRentIndex")]
        public async Task<IActionResult> Index()
        {
            var OwnerRentData = await ownerRentRepo.GetAllAsync(c => c.Car, c => c.CarOwner, c => c.Stock);
            var ownerRentGetDto = _mapper.Map<List<OwnerRentGetDto>>(OwnerRentData);
            var Cars = await carRepo.GetAllAsync();
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerRentRegisterDto = new OwnerRentRegisterDto
            {
                Cars = _mapper.Map<List<DrpDto>>(Cars),
                CarOwners = _mapper.Map<List<DrpDto>>(CarOwners),
                Stocks = _mapper.Map<List<DrpDto>>(Stocks)
            };
            var ownerRentModelDto = new OwnerRentModelDto
            {
                OwnerRentGetDtos = ownerRentGetDto,
                OwnerRentRegisterDto = ownerRentRegisterDto
            };
            return View(ownerRentModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerRentCreate")]

        public async Task<IActionResult> Create(OwnerRentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var OwnerRentDb = _mapper.Map<OwnerRentContract>(model);
                OwnerRentDb.CreatedDate = DateTime.Now;
                OwnerRentDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                ownerRentRepo.Add(OwnerRentDb);
                var carOwner = carOwnerRepo.SingleOrDefaultAsync(c => c.Id == OwnerRentDb.CarOwnerId).Result.name;

                StockMovement stockMovement = new StockMovement
                {
                    MovementId = OwnerRentDb.Id,
                    MovementType = StockMovementType.OwnerRent,
                    StockId = OwnerRentDb.StockId,
                    Date = OwnerRentDb.Date,
                    OutValue = OwnerRentDb.Payment,
                    InValue = 0,
                    Comment = " دفعة من فاتوره تاجير المالك" + " " + carOwner,
                    Notes = OwnerRentDb.Notes
                };
                stockMovementRepo.Add(stockMovement);

                CarOwnerAccount carOwnerAccount = new CarOwnerAccount
                {
                    MovementId = OwnerRentDb.Id,
                    AccountType = RentAccountType.Rent,
                    Dept = OwnerRentDb.TotalValue,
                    Borrower = OwnerRentDb.Payment,

                    Date = OwnerRentDb.Date,
                    Notes = OwnerRentDb.Notes,
                    CarOwnerId = OwnerRentDb.CarOwnerId,
                    Explain = "  فاتوره تاجير المالك" + " " + carOwner
                };
                carOwnerAccountRepo.Add(carOwnerAccount);

                await ownerRentRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var OwnerRentData = await ownerRentRepo.GetAllAsync(c => c.Car, c => c.CarOwner, c => c.Stock);
                var ownerRentGetDto = _mapper.Map<List<OwnerRentGetDto>>(OwnerRentData);
                var Cars = await carRepo.GetAllAsync();
                var CarOwners = await carOwnerRepo.GetAllAsync();
                var Stocks = await stockRepo.GetAllAsync();
                var ownerRentRegisterDto = _mapper.Map<OwnerRentRegisterDto>(model);
                ownerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                ownerRentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
                ownerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var ownerRentModelDto = new OwnerRentModelDto
                {
                    OwnerRentGetDtos = ownerRentGetDto,
                    OwnerRentRegisterDto = ownerRentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", ownerRentModelDto);
            }
        }


        [Authorize("Permissions.OwnerRentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var OwnerRentById = await ownerRentRepo.GetByIdAsync(id);
            if (OwnerRentById == null)
                return NotFound();
            var Cars = await carRepo.GetAllAsync();
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerRentRegisterDto = _mapper.Map<OwnerRentRegisterDto>(OwnerRentById);
            ownerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
            ownerRentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
            ownerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialOwnerRent", ownerRentRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.OwnerRentEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var OwnerRentById = await ownerRentRepo.GetByIdAsync(id);
            if (OwnerRentById == null)
                return NotFound();
            var OwnerRentData = await ownerRentRepo.GetAllAsync(c => c.Car, c => c.CarOwner, c => c.Stock);
            var ownerRentGetDto = _mapper.Map<List<OwnerRentGetDto>>(OwnerRentData);
            var Cars = await carRepo.GetAllAsync();
            var CarOwners = await carOwnerRepo.GetAllAsync();
            var Stocks = await stockRepo.GetAllAsync();
            var ownerRentRegisterDto = _mapper.Map<OwnerRentRegisterDto>(OwnerRentById);
            ownerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
            ownerRentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
            ownerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            var ownerRentModelDto = new OwnerRentModelDto
            {
                OwnerRentGetDtos = ownerRentGetDto,
                OwnerRentRegisterDto = ownerRentRegisterDto
            };
            return View("Index", ownerRentModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.OwnerRentEdit")]
        public async Task<IActionResult> Edit(OwnerRentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var OwnerRentById = await ownerRentRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = OwnerRentById.CreatedUser;
                var userAddDate = OwnerRentById.CreatedDate;
                var OwnerRentDb = _mapper.Map(model, OwnerRentById);
                OwnerRentDb.CreatedDate = userAddDate;
                OwnerRentDb.CreatedUser = userAdd;
                OwnerRentDb.LastEditDate = DateTime.Now;
                OwnerRentDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                ownerRentRepo.Update(OwnerRentDb);
                var carOwner = carOwnerRepo.SingleOrDefaultAsync(c => c.Id == OwnerRentDb.CarOwnerId).Result.name;

                var StockMovementById = stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == model.Id && s.MovementType == StockMovementType.OwnerRent).Result;

                StockMovementById.StockId = model.StockId;
                StockMovementById.Date = (DateTime)model.Date;
                StockMovementById.OutValue = (double)model.Payment;
                StockMovementById.InValue = 0;
                StockMovementById.Comment = " دفعة من فاتوره تاجير المالك" + " " + carOwner;
                StockMovementById.Notes = model.Notes;

                stockMovementRepo.Update(StockMovementById);
                var CarOwnerAccountById = carOwnerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == model.Id && c.AccountType == RentAccountType.Rent).Result;

                CarOwnerAccountById.Dept = model.TotalValue;
                CarOwnerAccountById.Borrower = (double)model.Payment;

                CarOwnerAccountById.Date = (DateTime)model.Date;
                CarOwnerAccountById.Notes = model.Notes;
                CarOwnerAccountById.CarOwnerId = model.CarOwnerId;
                CarOwnerAccountById.Explain = "  فاتوره تاجير المالك" + " " + carOwner;

                carOwnerAccountRepo.Update(CarOwnerAccountById);

                await ownerRentRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var OwnerRentData = await ownerRentRepo.GetAllAsync(c => c.Car, c => c.CarOwner, c => c.Stock);
                var ownerRentGetDto = _mapper.Map<List<OwnerRentGetDto>>(OwnerRentData);
                var Cars = await carRepo.GetAllAsync();
                var CarOwners = await carOwnerRepo.GetAllAsync();
                var Stocks = await stockRepo.GetAllAsync();
                var ownerRentRegisterDto = _mapper.Map<OwnerRentRegisterDto>(model);

                ownerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                ownerRentRegisterDto.CarOwners = _mapper.Map<List<DrpDto>>(CarOwners);
                ownerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);

                var ownerRentModelDto = new OwnerRentModelDto
                {
                    OwnerRentGetDtos = ownerRentGetDto,
                    OwnerRentRegisterDto = ownerRentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", ownerRentModelDto);

            }
        }

        [Authorize("Permissions.OwnerRentDelete")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var OwnerRentById = await ownerRentRepo.GetByIdAsync(id);
            if (OwnerRentById == null)
                return NotFound();
            var StockMovementById =await stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == id && s.MovementType == StockMovementType.OwnerRent);
            var CarOwnerAccountById =await carOwnerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == id && c.AccountType == RentAccountType.Rent);

            stockMovementRepo.Delete(StockMovementById);
            carOwnerAccountRepo.Delete(CarOwnerAccountById);
            ownerRentRepo.Delete(OwnerRentById);
            await ownerRentRepo.SaveAllAsync();
            return Ok();
        }


        public IActionResult GetMounthValue(Guid id)
        {
            var MounthValue = carRepo.SingleOrDefaultAsync(c => c.Id == id).Result.PriceRentOwnerPerMonth;
            return Json(MounthValue);
        }

        public IActionResult GetOwnerCars(Guid id)
        {
            var CarsOfOwner = carRepo.GetAllAsync(c => c.CarOwnertId == id).Result.Select(c => new { id = c.Id, name = c.name });
            return Json(CarsOfOwner);
        }

    }
}
