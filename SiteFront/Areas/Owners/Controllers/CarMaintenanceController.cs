using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CarMaintenanceDto;
using Core.Dtos.ExpenseDto;
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
    public class CarMaintenanceController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CarMaintenance> _maintenanceRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<StockMovement> _stockMovementRepo;
        private readonly UserManager<User> _userManager;

        public CarMaintenanceController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CarMaintenance> maintenanceRepo,
            IRepository<Car> carRepo,
            IRepository<Stock> stockRepo,
            IRepository<StockMovement> stockMovementRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _maintenanceRepo = maintenanceRepo;
            _carRepo = carRepo;
            _stockRepo = stockRepo;
            _stockMovementRepo = stockMovementRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CarMaintenanceIndex")]
        public async Task<IActionResult> Index()
        {
            var CarMaintenanceData = await _maintenanceRepo.GetAllAsync(m => m.Car, m => m.Stock);
            var carMaintenanceGetDto = _mapper.Map<List<CarMaintenanceGetDto>>(CarMaintenanceData);
            var Cars =await _carRepo.GetAllAsync();
            var Stocks =await _stockRepo.GetAllAsync();
            var carMaintenanceRegisterDto = new CarMaintenanceRegisterDto
            {
                Cars = _mapper.Map<List<DrpDto>>(Cars),
                Stocks = _mapper.Map<List<DrpDto>>(Stocks)
            };
            var carMaintenanceModelDto = new CarMaintenanceModelDto
            {
                CarMaintenanceGetDtos = carMaintenanceGetDto,
                CarMaintenanceRegisterDto = carMaintenanceRegisterDto
            };
            return View(carMaintenanceModelDto);
        }

        //[Authorize("Permissions.CarMaintenanceIndex")]
        //public async Task<IActionResult> CarMaintenanceData()
        //{
        //    var CarMaintenanceData =_maintenanceRepo.GetAllAsync(m => m.Car, m => m.Stock).Result.Select(m => new CarMaintenanceDataDto
        //    {
        //        CarName=m.Car.name,
        //        StockName=m.Stock.name,
        //        date=m.date,
        //        value=m.value,
        //        Notes=m.Notes,
        //        CreatedUser = m.CreatedUser == null ? m.CreatedUser : _userManager.FindByIdAsync(m.CreatedUser).Result.Name,
        //        CreatedDate = m.CreatedDate,
        //        LastEditUser = m.LastEditUser == null ? m.LastEditUser : _userManager.FindByIdAsync(m.LastEditUser).Result.Name,
        //        LastEditDate = m.LastEditDate
             
        //    });

        //    return View(CarMaintenanceData);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CarMaintenanceCreate")]
        public async Task<IActionResult> Create(CarMaintenanceRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarMaintenanceDb = _mapper.Map<CarMaintenance>(model);
                CarMaintenanceDb.CreatedDate = DateTime.Now;
                CarMaintenanceDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _maintenanceRepo.Add(CarMaintenanceDb);

                var car = _carRepo.SingleOrDefaultAsync(c => c.Id == CarMaintenanceDb.CarId).Result.name;
                var StockMovement = new StockMovement
                {
                    MovementId = CarMaintenanceDb.Id,
                    MovementType = StockMovementType.CarMaintenance,
                    Date = CarMaintenanceDb.date,
                    InValue = 0,
                    OutValue = CarMaintenanceDb.value,
                    Notes = CarMaintenanceDb.Notes,
                    Comment = "مصاريف صيانة السيارة" + " " + car,
                    StockId= CarMaintenanceDb.StockId
                };
                _stockMovementRepo.Add(StockMovement);
                await _maintenanceRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تمت الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var CarMaintenanceData = await _maintenanceRepo.GetAllAsync(m => m.Car, m => m.Stock);
                var carMaintenanceGetDto = _mapper.Map<List<CarMaintenanceGetDto>>(CarMaintenanceData);
                var Cars = await _carRepo.GetAllAsync();
                var Stocks = await _stockRepo.GetAllAsync();
                var carMaintenanceRegisterDto = _mapper.Map<CarMaintenanceRegisterDto>(model);
                carMaintenanceRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                carMaintenanceRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var carMaintenanceModelDto = new CarMaintenanceModelDto
                {
                    CarMaintenanceGetDtos = carMaintenanceGetDto,
                    CarMaintenanceRegisterDto = carMaintenanceRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",carMaintenanceModelDto);
            }
        }

        [Authorize("Permissions.CarMaintenanceEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CarMaintenanceById = await _maintenanceRepo.GetByIdAsync(id);
            var Cars = await _carRepo.GetAllAsync();
            var Stocks = await _stockRepo.GetAllAsync();
            var carMaintenanceRegisterDto = _mapper.Map<CarMaintenanceRegisterDto>(CarMaintenanceById);
            carMaintenanceRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
            carMaintenanceRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialCarMaintenance", carMaintenanceRegisterDto);
        }

        //For Movement And Account
        [Authorize("Permissions.CarMaintenanceEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CarMaintenanceById = await _maintenanceRepo.GetByIdAsync(id);
            var CarMaintenanceData = await _maintenanceRepo.GetAllAsync(m => m.Car, m => m.Stock);
            var carMaintenanceGetDto = _mapper.Map<List<CarMaintenanceGetDto>>(CarMaintenanceData);
            var Cars = await _carRepo.GetAllAsync();
            var Stocks = await _stockRepo.GetAllAsync();
            var carMaintenanceRegisterDto = _mapper.Map<CarMaintenanceRegisterDto>(CarMaintenanceById);
            carMaintenanceRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
            carMaintenanceRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            var carMaintenanceModelDto = new CarMaintenanceModelDto
            {
                CarMaintenanceGetDtos = carMaintenanceGetDto,
                CarMaintenanceRegisterDto = carMaintenanceRegisterDto
            };
            return View("Index", carMaintenanceModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CarMaintenanceEdit")]
        public async Task<IActionResult> Edit(CarMaintenanceRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarMaintenanceById = await _maintenanceRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = CarMaintenanceById.CreatedUser;
                var userAddDate = CarMaintenanceById.CreatedDate;
                var CarMaintenanceDb = _mapper.Map(model, CarMaintenanceById);
                CarMaintenanceDb.CreatedDate = userAddDate;
                CarMaintenanceDb.CreatedUser = userAdd;
                CarMaintenanceDb.LastEditDate = DateTime.Now;
                CarMaintenanceDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _maintenanceRepo.Update(CarMaintenanceDb);

                var car = _carRepo.SingleOrDefaultAsync(c => c.Id == CarMaintenanceDb.CarId).Result.name;
                var StockMovementById =await _stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == CarMaintenanceDb.Id && s.MovementType == StockMovementType.CarMaintenance);

                StockMovementById.MovementId = CarMaintenanceDb.Id;
                StockMovementById.MovementType = StockMovementType.CarMaintenance;
                StockMovementById.Date = CarMaintenanceDb.date;
                StockMovementById.InValue = 0;
                StockMovementById.OutValue = CarMaintenanceDb.value;
                StockMovementById.Notes = CarMaintenanceDb.Notes;
                StockMovementById.Comment = "مصاريف صيانة السيارة" + " " + car;
                StockMovementById.StockId = CarMaintenanceDb.StockId;

                _stockMovementRepo.Update(StockMovementById);
                await _maintenanceRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم التعديل ");
                return RedirectToAction("Index");
            }
            else
            {
                var CarMaintenanceData = await _maintenanceRepo.GetAllAsync(m => m.Car, m => m.Stock);
                var carMaintenanceGetDto = _mapper.Map<List<CarMaintenanceGetDto>>(CarMaintenanceData);
                var Cars = await _carRepo.GetAllAsync();
                var Stocks = await _stockRepo.GetAllAsync();
                var carMaintenanceRegisterDto = _mapper.Map<CarMaintenanceRegisterDto>(model);
                carMaintenanceRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                carMaintenanceRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var carMaintenanceModelDto = new CarMaintenanceModelDto
                {
                    CarMaintenanceGetDtos = carMaintenanceGetDto,
                    CarMaintenanceRegisterDto = carMaintenanceRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",carMaintenanceModelDto);
            }
        }

        [Authorize("Permissions.CarMaintenanceDelete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var CarMaintenanceById = await _maintenanceRepo.GetByIdAsync(id);
            if (CarMaintenanceById == null)
                return NotFound();
            var StockMovementById = await _stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == id&& s.MovementType == StockMovementType.CarMaintenance);
            _stockMovementRepo.Delete(StockMovementById);
            _maintenanceRepo.Delete(CarMaintenanceById);
            await _maintenanceRepo.SaveAllAsync();
            return Ok();
        }
    }
}
