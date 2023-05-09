using AutoMapper;
using Core.Common.enums;
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

namespace SiteFront.Areas.Account.Controllers
{
    [Area("Account")]
    //[AllowAnonymous]
    public class ExpensesController : Controller
    {

        private readonly IMapper _Mapper;
        private readonly IRepository<ExpenseType> _ExpenseTypeRepo;
         private readonly IRepository<Expense> _ExpenseRepo;
         private readonly IRepository<Stock> _StockRepo;
         private readonly IRepository<StockMovement> _StockMovementRepo;
         private readonly IToastNotification _ToastNotification;
        private readonly UserManager<User> _userManager;

        public ExpensesController(IMapper mapper,
            IRepository<ExpenseType> ExpenseTypeRepo,
            IRepository<Expense> ExpenseRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IToastNotification toastNotification,
            UserManager<User> userManager
          )
        {
            _Mapper = mapper;
            _ExpenseTypeRepo = ExpenseTypeRepo;
            _ExpenseRepo = ExpenseRepo;
            _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _ToastNotification = toastNotification;
            _userManager = userManager;
        }


        public List<ExpenseGetDto> AllExpensesModel { get; set; }
        public ExpenseRegisterDto ExpenseRegistermodel { get; set; }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.ExpensesIndex")]
        public async Task<IActionResult> Index()
        {
            var AllExpences = await _ExpenseRepo.GetAllAsync(n=>n.Stock,n=>n.ExpenseType);
            AllExpensesModel = _Mapper.Map<List<ExpenseGetDto>>(AllExpences);
            var Expenses = await _ExpenseTypeRepo.GetAllAsync();
            var ExpensesMapping = _Mapper.Map<List<DrpDto>>(Expenses);
            var expenseReportDto = new ExpenseReportDto
            {
                DrpExpenseTypeDto = ExpensesMapping
            };
            var expenseModelDto = new ExpenseModelDto
            {
                ExpenseGetDtos = AllExpensesModel,
                ExpenseReportDto = expenseReportDto
            };
            return View(expenseModelDto);
        }

        [Authorize("Permissions.ExpensesIndex")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ExpenseModelDto model)
        {
            if (ModelState.IsValid)
            {
                var expense = _ExpenseRepo.GetAllAsync(e=>e.ExpenseTypeId==model.ExpenseReportDto.ExpenseTypeId,e=>e.ExpenseType,e=>e.Stock).Result
                                            .Where(s => s.Date >= model.ExpenseReportDto.FromDate)
                                            .Where(s => s.Date <= model.ExpenseReportDto.ToDate);
                var expenseGetDto = _Mapper.Map<List<ExpenseGetDto>>(expense);
                var Expenses = await _ExpenseTypeRepo.GetAllAsync();
                var ExpensesMapping = _Mapper.Map<List<DrpDto>>(Expenses);
                var expenseReportDto = new ExpenseReportDto
                {
                    DrpExpenseTypeDto = ExpensesMapping
                };
                var expenseModelDto = new ExpenseModelDto
                {
                    ExpenseGetDtos = expenseGetDto,
                    ExpenseReportDto = expenseReportDto
                };
                return View("Index", expenseModelDto);
            }
            return BadRequest();

        }

        [Authorize("Permissions.ExpensesIndex")]
        [HttpGet]
        public async Task<IActionResult> Create(Guid ? Id)
        {
            var Stocks = await _StockRepo.GetAllAsync();
            var Expenses = await _ExpenseTypeRepo.GetAllAsync();
            var StocksMode= _Mapper.Map<List<DrpDto>>(Stocks);
            var ExpensesMode = _Mapper.Map<List<DrpDto>>(Expenses);

            if (Id == null)
            {
                ExpenseRegistermodel = new ExpenseRegisterDto();
            }
            else
            {
                var ExpensesById = await _ExpenseRepo.GetByIdAsync((Guid)Id);
                ExpenseRegistermodel = _Mapper.Map<ExpenseRegisterDto>(ExpensesById);

            }

            ExpenseRegistermodel.DrpExpenseTypeDto = ExpensesMode;
            ExpenseRegistermodel.DrpstockDto = StocksMode;

            return View(ExpenseRegistermodel);
        }

        [Authorize("Permissions.ExpensesCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ExpenseRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var ExpensesDb = _Mapper.Map<Expense>(model);
                ExpensesDb.CreatedDate = DateTime.Now;
                ExpensesDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _ExpenseRepo.Add(ExpensesDb);


                var ExpensesName = _ExpenseTypeRepo.SingleOrDefaultAsync(n=>n.Id== model.ExpenseTypeId).Result.name;

                StockMovement StockMovement = new StockMovement()
                {
                    MovementId=ExpensesDb.Id,
                    MovementType = StockMovementType.Expense,
                    StockId = ExpensesDb.StockId,
                    Date = ExpensesDb.Date,
                   OutValue = ExpensesDb.Value,
                    InValue = 0,
                    Notes = ExpensesDb.Notes,
                    Comment = ExpensesName +" " + ExpensesDb.Value,
                   
                };

                _StockMovementRepo.Add(StockMovement);

                await _ExpenseRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تمت الاضافة");

                return RedirectToAction("Index");

            }
            else
            {
                var Stocks = await _StockRepo.GetAllAsync();
                var Expenses = await _ExpenseTypeRepo.GetAllAsync();
                var StocksMode = _Mapper.Map<List<DrpDto>>(Stocks);
                var ExpensesMode = _Mapper.Map<List<DrpDto>>(Expenses);
                model.DrpExpenseTypeDto = ExpensesMode;
                model.DrpstockDto = StocksMode;
               return View(model);

            }



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.ExpensesEdit")]

        public async Task<IActionResult> Edit(ExpenseRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var ExpensesById = await _ExpenseRepo.GetByIdAsync((Guid)model.Id);

                if (ExpensesById == null)
                    return NotFound();
                var userAdd = ExpensesById.CreatedUser;
                var userAddDate = ExpensesById.CreatedDate;

                var ExpensisEditedDb = _Mapper.Map(model, ExpensesById);
                ExpensisEditedDb.CreatedDate = userAddDate;
                ExpensisEditedDb.CreatedUser = userAdd;
                ExpensisEditedDb.LastEditDate = DateTime.Now;
                ExpensisEditedDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                _ExpenseRepo.Update(ExpensisEditedDb);

                var ExpensesName = _ExpenseTypeRepo.SingleOrDefaultAsync(n => n.Id == model.ExpenseTypeId).Result.name;

                var StockMovementById = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == (Guid)model.Id && n.MovementType == StockMovementType.Expense);

                StockMovementById.Date = ExpensesById.Date;
                StockMovementById.StockId = ExpensesById.StockId;
                StockMovementById.Date = ExpensesById.Date;
                StockMovementById.OutValue = ExpensesById.Value;
                StockMovementById.Notes = ExpensesById.Notes;
                StockMovementById.Comment = ExpensesName + " " + ExpensesById.Value;
                 
               _StockMovementRepo.Update(StockMovementById);
             
                await _ExpenseRepo.SaveAllAsync();
                _ToastNotification.AddSuccessToastMessage("تم التعديل");

                return RedirectToAction("Index");

            }
            else
            {


                var Stocks = await _StockRepo.GetAllAsync();
                var Expenses = await _ExpenseTypeRepo.GetAllAsync();
                var StocksMode = _Mapper.Map<List<DrpDto>>(Stocks);
                var ExpensesMode = _Mapper.Map<List<DrpDto>>(Expenses);
                model.DrpExpenseTypeDto = ExpensesMode;
                model.DrpstockDto = StocksMode;
                return View(model);

            }

        }
        [Authorize("Permissions.ExpensesDelete")]

        public async Task<IActionResult> Delete(Guid Id)
        {
            var StockMovement = await _StockMovementRepo.SingleOrDefaultAsync(n => n.MovementId == Id && n.MovementType == StockMovementType.Expense);
            _StockMovementRepo.Delete(StockMovement);

            var ExpenseById = await _ExpenseRepo.GetByIdAsync(Id);
            _ExpenseRepo.Delete(ExpenseById);

           await _ExpenseRepo.SaveAllAsync();
          _ToastNotification.AddSuccessToastMessage("تم الحذف");
           
           return RedirectToAction("Index");
        }


    }
}
