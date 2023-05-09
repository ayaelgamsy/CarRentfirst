using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerDto;
using Core.Dtos.CustomerRentDto;
using Core.Dtos.EmployeeDto;
using Core.Dtos.ExpenseDto;
using Core.Dtos.MarketerDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SiteFront.Areas.Rent.Controllers
{
    [Area("Rent")]
   // [AllowAnonymous]
    public class CustomerRentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<CustomerRent> _CustomerRentRepo;
        private readonly IWebHostEnvironment _hosting;
        private readonly IRepository<GuarantorPhoto> _guarantorPhotoRepo;
        private readonly IRepository<Customer> _custRepo;
        private readonly IRepository<Employee> _emplRepo;
        private readonly IRepository<Marketer> _marketerRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Stock> _StockRepo;
        private readonly IRepository<StockMovement> _StockMovementRepo;
        private readonly IRepository<CustomerAccount> _CustomerAccountRepo;
        private readonly IToastNotification _toastNotification;
        private readonly UserManager<User> _userManager;

        public CustomerRentController(IMapper mapper, 
            IRepository<CustomerRent> custRentRepo,
            IWebHostEnvironment hosting,
            IRepository<GuarantorPhoto> GuarantorPhotoRepo,
            IRepository<Customer> custRepo,
            IRepository<Employee> emplRepo, 
            IRepository<Marketer> marketerRepo,
            IRepository<Car> carRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<CustomerAccount> CustomerAccountRepo, 
            IToastNotification toastNotification,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _CustomerRentRepo = custRentRepo;
            _hosting = hosting;
            _guarantorPhotoRepo = GuarantorPhotoRepo;
            _custRepo = custRepo;
            _emplRepo = emplRepo;
            _marketerRepo = marketerRepo;
            _carRepo = carRepo;
            _StockRepo = StockRepo;
            _StockMovementRepo = StockMovementRepo;
            _CustomerAccountRepo = CustomerAccountRepo;
            _toastNotification = toastNotification;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomerRentIndex")]
        public async Task<IActionResult> Index()
        {
            var customersRent = await _CustomerRentRepo.GetAllAsync(c => c.Car, e => e.Employee, c => c.Customer, c=>c.Marketer, c => c.Stock, c => c.GuarantorPhotos);
            var customersRentData = _mapper.Map<List<CustomerRentGetDto>>(customersRent);
            var Cars = await (_carRepo.GetAll(n => n.Active == true ).AsNoTracking()).ToListAsync();
            var Customers = await _custRepo.GetAllAsync(true);
            var Employees = await _emplRepo.GetAllAsync(true);
            var Marketers = await _marketerRepo.GetAllAsync(true);
            var Stocks = await _StockRepo.GetAllAsync(true);
            var customerRentRegisterDto = new CustomerRentRegisterDto
            {
                Cars = _mapper.Map<List<DrpDto>>(Cars),
                Customers = _mapper.Map<List<DrpDto>>(Customers),
                Employees = _mapper.Map<List<DrpDto>>(Employees),
                Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers),
                Stocks = _mapper.Map<List<DrpDto>>(Stocks)
            };
            var customerRentModelDto = new CustomerRentModelDto
            {
                CustomerRentGetDtos = customersRentData,
                CustomerRentRegisterDto = customerRentRegisterDto
            };
            return View(customerRentModelDto);
        }

        [Authorize("Permissions.CustomerRentIndex")]
        public async Task<IActionResult> RentIndex(Guid id)
        {
            var carById = await _carRepo.GetByIdAsync(id);
            var customersRent = await _CustomerRentRepo.GetAllAsync(c => c.Car, e => e.Employee, c => c.Customer, c => c.Marketer, c => c.Stock, c => c.GuarantorPhotos);
            var customersRentData = _mapper.Map<List<CustomerRentGetDto>>(customersRent);
            var Cars = await (_carRepo.GetAll(n => n.Active == true).AsNoTracking()).ToListAsync();
            var Customers = await _custRepo.GetAllAsync(true);
            var Employees = await _emplRepo.GetAllAsync(true);
            var Marketers = await _marketerRepo.GetAllAsync(true);
            var Stocks = await _StockRepo.GetAllAsync(true);
            var customerRentRegisterDto = new CustomerRentRegisterDto
            {
                Cars = _mapper.Map<List<DrpDto>>(Cars),
                Customers = _mapper.Map<List<DrpDto>>(Customers),
                Employees = _mapper.Map<List<DrpDto>>(Employees),
                Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers),
                Stocks = _mapper.Map<List<DrpDto>>(Stocks)
            };
            customerRentRegisterDto.CarId = id;
            customerRentRegisterDto.PricePerDay = carById.PriceOfDaye;
            var customerRentModelDto = new CustomerRentModelDto
            {
                CustomerRentGetDtos = customersRentData,
                CustomerRentRegisterDto = customerRentRegisterDto
            };
            return View("Index",customerRentModelDto);
        }


        [Authorize("Permissions.CustomerRentCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerRentRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var customersRent = await _CustomerRentRepo.GetAllAsync(c => c.Car, e => e.Employee, c => c.Customer, c => c.Marketer, c => c.Stock, c => c.GuarantorPhotos);
                var customersRentData = _mapper.Map<List<CustomerRentGetDto>>(customersRent);
                var Cars = await (_carRepo.GetAll(n => n.Active == true ).AsNoTracking()).ToListAsync();
                var Customers = await _custRepo.GetAllAsync(true);
                var Employees = await _emplRepo.GetAllAsync(true);
                var Marketers = await _marketerRepo.GetAllAsync(true);
                var Stocks = await _StockRepo.GetAllAsync(true);
                var customerRentRegisterDto = _mapper.Map<CustomerRentRegisterDto>(model);
                customerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                customerRentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(Customers);
                customerRentRegisterDto.Employees = _mapper.Map<List<DrpDto>>(Employees);
                customerRentRegisterDto.Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers);
                customerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var customerRentModelDto = new CustomerRentModelDto
                {
                    CustomerRentGetDtos = customersRentData,
                    CustomerRentRegisterDto = customerRentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",customerRentModelDto);
            }
            if(model.Id!=null)
            {
                model.Id = Guid.NewGuid();
            }
            var newCustomerRent = _mapper.Map<CustomerRent>(model);
            newCustomerRent.CreatedDate = DateTime.Now;
            newCustomerRent.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
            _CustomerRentRepo.Add(newCustomerRent);

            var CareDb = await _carRepo.GetByIdAsync(newCustomerRent.CarId);
            CareDb.Available = false;
            CareDb.EndTripDate = newCustomerRent.EndDate;
            var customer = _custRepo.SingleOrDefaultAsync(c => c.Id == newCustomerRent.CustomerId).Result.name;

            StockMovement stockMovement = new StockMovement
            {
                MovementId = newCustomerRent.Id,
                MovementType= StockMovementType.CustomerRent,
                StockId = newCustomerRent.StockId,
                Date = newCustomerRent.StartDate,
                InValue = newCustomerRent.payment,
                OutValue = 0,
                Comment = "دفعة من فاتوره تاجير للعميل" + " " + customer,
                Notes = newCustomerRent.Notes
            };
            _StockMovementRepo.Add(stockMovement);

            CustomerAccount customerAccount = new CustomerAccount
            {
                CustomerId = newCustomerRent.CustomerId,
                Date = newCustomerRent.StartDate,
                Borrower = newCustomerRent.payment,
                Dept = newCustomerRent.Total,
                Explain = "فاتورة تاجير للعميل" + " " + customer,
                Notes = newCustomerRent.Notes,
                MovementId = newCustomerRent.Id,
                AccountType= RentAccountType.Rent
            };
            _CustomerAccountRepo.Add(customerAccount);
            await _CustomerRentRepo.SaveAllAsync();

            //For Photos
            if (model.GuarantorPhotoFile != null)
            {
                var upload = Path.Combine(_hosting.WebRootPath+ "/Images/Guarantors/" + newCustomerRent.Id.ToString());
                if (!Directory.Exists(upload))
                    Directory.CreateDirectory(upload);
                foreach (IFormFile item in model.GuarantorPhotoFile)
                {
                    var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                    var FullPath = Path.Combine(upload, fileName);

                    string fileExtention = item.ContentType;
                    var fileLenght = item.Length;
                    if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                    {
                        if (fileLenght >= 3145728)
                        {
                            Bitmap bmpPostedImage = new Bitmap(item.OpenReadStream());
                            Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                            // Saving image in jpeg format
                            objImage.Save(FullPath, ImageFormat.Jpeg);
                        }
                        else
                        {
                            item.CopyTo(new FileStream(FullPath, FileMode.Create));
                        }
                    }
                    else
                    {
                        item.CopyTo(new FileStream(FullPath, FileMode.Create));
                    }
                    var GuarantorPhoto = new GuarantorPhoto
                    {
                        PhotoUrl = fileName,
                        CustomerRentId = newCustomerRent.Id
                    };
                    _guarantorPhotoRepo.Add(GuarantorPhoto);
                    await _guarantorPhotoRepo.SaveAllAsync();
                }
            } 

            _toastNotification.AddSuccessToastMessage("تم الحفظ");
            return RedirectToAction(nameof(Index));
        }

        [Authorize("Permissions.CustomerRentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var customerRent = await _CustomerRentRepo.GetByIdAsync(id);
            if (customerRent == null)
                return NotFound();
            var Cars = await _carRepo.GetAllAsync(true);
            var Customers = await _custRepo.GetAllAsync(true);
            var Employees = await _emplRepo.GetAllAsync(true);
            var Marketers = await _marketerRepo.GetAllAsync(true);
            var Stocks = await _StockRepo.GetAllAsync(true);
            var customerRentRegister = _mapper.Map<CustomerRentRegisterDto>(customerRent);
            customerRentRegister.Cars = _mapper.Map<List<DrpDto>>(Cars);
            customerRentRegister.Customers = _mapper.Map<List<DrpDto>>(Customers);
            customerRentRegister.Employees = _mapper.Map<List<DrpDto>>(Employees);
            customerRentRegister.Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers);
            customerRentRegister.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            return PartialView("_PartialCustomerRent", customerRentRegister);
        }

        //For Movement And Account 
        [HttpGet]
        [Authorize("Permissions.CustomerRentEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var customerRentById = await _CustomerRentRepo.GetByIdAsync(id);
            if (customerRentById == null)
                return NotFound();
            var customersRent = await _CustomerRentRepo.GetAllAsync(c => c.Car, e => e.Employee, c => c.Customer, c => c.Marketer, c => c.Stock, c => c.GuarantorPhotos);
            var customersRentData = _mapper.Map<List<CustomerRentGetDto>>(customersRent);
            var Cars = await _carRepo.GetAllAsync(true);
            var Customers = await _custRepo.GetAllAsync(true);
            var Employees = await _emplRepo.GetAllAsync(true);
            var Marketers = await _marketerRepo.GetAllAsync(true);
            var Stocks = await _StockRepo.GetAllAsync(true);
            var customerRentRegisterDto = _mapper.Map<CustomerRentRegisterDto>(customerRentById);
            customerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
            customerRentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(Customers);
            customerRentRegisterDto.Employees = _mapper.Map<List<DrpDto>>(Employees);
            customerRentRegisterDto.Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers);
            customerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
            var customerRentModelDto = new CustomerRentModelDto
            {
                CustomerRentGetDtos = customersRentData,
                CustomerRentRegisterDto = customerRentRegisterDto
            };
            return View("Index", customerRentModelDto);
        }


        [Authorize("Permissions.CustomerRentEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerRentRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                var customersRent = await _CustomerRentRepo.GetAllAsync(c => c.Car, e => e.Employee, c => c.Customer, c => c.Marketer, c => c.Stock, c => c.GuarantorPhotos);
                var customersRentData = _mapper.Map<List<CustomerRentGetDto>>(customersRent);
                var Cars = await _carRepo.GetAllAsync(true);
                var Customers = await _custRepo.GetAllAsync(true);
                var Employees = await _emplRepo.GetAllAsync(true);
                var Marketers = await _marketerRepo.GetAllAsync(true);
                var Stocks = await _StockRepo.GetAllAsync(true);
                var customerRentRegisterDto = _mapper.Map<CustomerRentRegisterDto>(model);
                customerRentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(Cars);
                customerRentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(Customers);
                customerRentRegisterDto.Employees = _mapper.Map<List<DrpDto>>(Employees);
                customerRentRegisterDto.Marketers = _mapper.Map<List<MarketerDropDto>>(Marketers);
                customerRentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(Stocks);
                var customerRentModelDto = new CustomerRentModelDto
                {
                    CustomerRentGetDtos = customersRentData,
                    CustomerRentRegisterDto = customerRentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", customerRentModelDto);
            }

            var editCustomerRent = await _CustomerRentRepo.GetByIdAsync( (Guid)model.Id );
            if (editCustomerRent == null)
                return NotFound();
            var userAdd = editCustomerRent.CreatedUser;
            var userAddDate = editCustomerRent.CreatedDate;
            if (editCustomerRent.CarId != model.CarId)
            {
                var lastCre = await _carRepo.GetByIdAsync(editCustomerRent.CarId);
                lastCre.Available = true;
                _carRepo.Update(lastCre);
            }
                var Credb = await _carRepo.GetByIdAsync(model.CarId);
                Credb.Available = false;
                Credb.EndTripDate = (DateTime) model.EndDate;
                _carRepo.Update(Credb);

            if (model.GuarantorPhotoFile == null)
            {
                model.GuarantorPhotos = editCustomerRent.GuarantorPhotos;
            }
            var CustomerRentMaped = _mapper.Map(model, editCustomerRent);
            CustomerRentMaped.CreatedDate = userAddDate;
            CustomerRentMaped.CreatedUser = userAdd;
            CustomerRentMaped.LastEditDate = DateTime.Now;
            CustomerRentMaped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
            _CustomerRentRepo.Update(CustomerRentMaped);

            var customer = _custRepo.SingleOrDefaultAsync(c => c.Id == CustomerRentMaped.CustomerId).Result.name;

            StockMovement stockMovement =await _StockMovementRepo.SingleOrDefaultAsync(c => c.MovementId == CustomerRentMaped.Id && c.MovementType==StockMovementType.CustomerRent );

                            stockMovement.StockId = CustomerRentMaped.StockId;
                            stockMovement.Date = CustomerRentMaped.StartDate;
                            stockMovement.InValue = CustomerRentMaped.payment;
                            stockMovement.OutValue = 0;
                            stockMovement.Comment = " دفعة من فاتوره تاجير للعميل  " + " " + customer;
                            stockMovement.Notes = CustomerRentMaped.Notes;           
            _StockMovementRepo.Update(stockMovement);

            CustomerAccount customerAccount = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == CustomerRentMaped.Id && c.AccountType==RentAccountType.Rent);

                            customerAccount.CustomerId = CustomerRentMaped.CustomerId;
                            customerAccount.Date = CustomerRentMaped.StartDate;
                            customerAccount.Borrower = CustomerRentMaped.payment;
                            customerAccount.Dept = CustomerRentMaped.Total;
                            customerAccount.Explain = " فاتورة تاجير للعميل" + " "  + customer;
                            customerAccount.Notes = CustomerRentMaped.Notes;
            _CustomerAccountRepo.Update(customerAccount);

            //For Photos
            if (model.GuarantorPhotoFile != null)
            {
                var oldUrlPhoto = await _guarantorPhotoRepo.GetAllAsync(p => p.CustomerRentId == editCustomerRent.Id);
                var upload = Path.Combine(_hosting.WebRootPath + "/Images/Guarantors/" + editCustomerRent.Id.ToString());

                foreach (IFormFile item in model.GuarantorPhotoFile)
                {
                    var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                    var FullPath = Path.Combine(upload, fileName);

                    string fileExtention = item.ContentType;
                    var fileLenght = item.Length;
                    if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                    {
                        if (fileLenght >= 3145728)
                        {
                            Bitmap bmpPostedImage = new Bitmap(item.OpenReadStream());
                            Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                            // Saving image in jpeg format
                            objImage.Save(FullPath, ImageFormat.Jpeg);
                        }
                        else
                        {
                            item.CopyTo(new FileStream(FullPath, FileMode.Create));
                        }
                    }
                    else
                    {
                        item.CopyTo(new FileStream(FullPath, FileMode.Create));
                    }
                    var GuarantorPhoto = new GuarantorPhoto
                    {
                        PhotoUrl = fileName,
                        CustomerRentId = editCustomerRent.Id
                    };
                    _guarantorPhotoRepo.Add(GuarantorPhoto);
                    await _guarantorPhotoRepo.SaveAllAsync();
                }
            }

            await _CustomerRentRepo.SaveAllAsync();
            _toastNotification.AddSuccessToastMessage("تم التعديل");
            return RedirectToAction("Index");
        }

        [Authorize("Permissions.CustomerRentDelete")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id, bool fromView = false)
        {
            var customerRent = await _CustomerRentRepo.SingleOrDefaultAsync(c => c.Id == id);
            if (customerRent == null)
                return NotFound();
            StockMovement stockMovement = await _StockMovementRepo.SingleOrDefaultAsync(c => c.MovementId == id&& c.MovementType==StockMovementType.CustomerRent);
            CustomerAccount customerAccount = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == id && c.AccountType==RentAccountType.Rent);

            _CustomerAccountRepo.Delete(customerAccount);
            _StockMovementRepo.Delete(stockMovement);
            _CustomerRentRepo.Delete(customerRent);

            await _CustomerRentRepo.SaveAllAsync();

            if (fromView)
            {
                _toastNotification.AddSuccessToastMessage("تم الحذف");
                return RedirectToAction(nameof(Index));
            }
            else
                return Ok();
        }


        [Authorize("Permissions.CustomerRentCreate")]
        // apis
        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetCarPrice(Guid codeId)
        {
            var car = await _carRepo.SingleOrDefaultAsync(c => c.Id == codeId, NoTacking: true);
            if (car == null)
                return NotFound();

            return Json(new { price = car.PriceOfDaye, minPrice = car.MinPriceOfDaye });
        }

        // GetEmployeeByPhone
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployeeByPhone(string phone)
        {
            var customer = await _custRepo.SingleOrDefaultAsync(c => c.phone1 == phone || c.phone2 == phone || c.phone3 == phone);
            if (customer == null)
                return NotFound();
            return Json(new { id = customer.Id });
        }


        //Get Employee Photos
        public IActionResult GetGuarantorPhotos(Guid id)
        {
            var GuarantorPhotos = _guarantorPhotoRepo.GetAllAsync(p => p.CustomerRentId == id, p => p.CustomerRent).Result;
            string FilePath = "/Images/Guarantors/" + id.ToString();

            return Json(GuarantorPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.CustomerRent.GuarantorName }));
        }


        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await _guarantorPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Guarantors/" + Photo.CustomerRentId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            _guarantorPhotoRepo.Delete(Photo);
            await _guarantorPhotoRepo.SaveAllAsync();

            return Ok();
        }

    }
}
