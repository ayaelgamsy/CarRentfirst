using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerViolationDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class CustomerViolationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _hosting;
        private readonly IRepository<CustomerViolation> _customerViolationRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<Stock> _stockRepo;
        private readonly IRepository<CustomerViolationPhoto> _customerViolationPhotoRepo;
        private readonly IRepository<CustomerAccount> _customerAccountRepo;
        private readonly IRepository<StockMovement> _stockmovementRepo;
        private readonly IRepository<CustomerRent> _customerRentRepo;
        private readonly UserManager<User> _userManager;

        public CustomerViolationController(IMapper mapper,
            IToastNotification toastNotification,
            IWebHostEnvironment hosting,
            IRepository<CustomerViolation> customerViolationRepo,
            IRepository<Car> carRepo,
            IRepository<Customer> customerRepo,
            IRepository<Stock> stockRepo,
            IRepository<CustomerViolationPhoto> customerViolationPhotoRepo,
            IRepository<CustomerAccount> customerAccountRepo,
            IRepository<StockMovement> stockmovementRepo,
            IRepository<CustomerRent> customerRentRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _hosting = hosting;
            _customerViolationRepo = customerViolationRepo;
            _carRepo = carRepo;
            _customerRepo = customerRepo;
            _stockRepo = stockRepo;
            _customerViolationPhotoRepo = customerViolationPhotoRepo;
            _customerAccountRepo = customerAccountRepo;
            _stockmovementRepo = stockmovementRepo;
            _customerRentRepo = customerRentRepo;
            _userManager = userManager;
        }


        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomerViolationIndex")]
        public async Task<IActionResult> Index()
        {
            var CustomerViolationData = await _customerViolationRepo.GetAllAsync(c=>c.Car,c=>c.Customer,c=>c.Stock,c=>c.CustomerViolationPhotos);
            var customerViolationGetDto = _mapper.Map<List<CustomerViolationGetDto>>(CustomerViolationData);
            var customerViolationRegisterDto = new CustomerViolationRegisterDto
            {
                Cars = await _carRepo.GetAllAsync(),
                Customers = await _customerRepo.GetAllAsync(),
                Stocks = await _stockRepo.GetAllAsync()
            };
            var CustomerViolationModelDto = new CustomerViolationModelDto
            {
                CustomerViolationGetDtos = customerViolationGetDto,
                CustomerViolationRegisterDto = customerViolationRegisterDto
            };
            return View(CustomerViolationModelDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerViolationCreate")]
        public async Task<IActionResult> Create(CustomerViolationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerViolationDb = _mapper.Map<CustomerViolation>(model);
                customerViolationDb.CreatedDate = DateTime.Now;
                customerViolationDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _customerViolationRepo.Add(customerViolationDb);

                var customer = _customerRepo.SingleOrDefaultAsync(c => c.Id == customerViolationDb.CustomerId).Result.name;
                var stockMovement = new StockMovement
                {
                    MovementId = customerViolationDb.Id,
                    MovementType = StockMovementType.CustomerViolation,
                    Date = customerViolationDb.Date,
                    InValue = customerViolationDb.Payment,
                    OutValue = 0,
                    Notes = customerViolationDb.Notes,
                    StockId = customerViolationDb.StockId,
                    Comment = " دفعة من مخالفة العميل" + " " + customer,
                };
                _stockmovementRepo.Add(stockMovement);
                var customerAccount = new CustomerAccount
                {
                    MovementId = customerViolationDb.Id,
                    AccountType = RentAccountType.Violation,
                    Date = customerViolationDb.Date,
                    Dept = customerViolationDb.Value,
                    Borrower = customerViolationDb.Payment,
                    Notes = customerViolationDb.Notes,
                    Explain = " دفعة من مخالفة العميل" + " " + customer,
                    CustomerId = customerViolationDb.CustomerId
                };
                _customerAccountRepo.Add(customerAccount);

                //For Photos
                if (model.ViolationPhotoFile != null)
                {
                    string upload = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerViolation/" + customerViolationDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.ViolationPhotoFile)
                    {
                        string fileName = Guid.NewGuid().ToString() + "." + Photo.FileName.Split(".")[1].ToString();
                        string fullPath = Path.Combine(upload, fileName);

                        string fileExtention = Photo.ContentType;
                        var fileLenght = Photo.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(Photo.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(fullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                        }
                        var customerViolationPhoto = new CustomerViolationPhoto
                        {
                            CustomerViolationId = customerViolationDb.Id,
                            PhotoUrl = fileName
                        };
                        _customerViolationPhotoRepo.Add(customerViolationPhoto);
                        await _customerViolationPhotoRepo.SaveAllAsync();
                    }
                }
                await _customerViolationRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerViolationData = await _customerViolationRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CustomerViolationPhotos);
                var customerViolationGetDto = _mapper.Map<List<CustomerViolationGetDto>>(CustomerViolationData);
                var customerViolationRegisterDto = _mapper.Map<CustomerViolationRegisterDto>(model);

                customerViolationRegisterDto.Cars = await _carRepo.GetAllAsync();
                customerViolationRegisterDto.Customers = await _customerRepo.GetAllAsync();
                customerViolationRegisterDto.Stocks = await _stockRepo.GetAllAsync();

                var CustomerViolationModelDto = new CustomerViolationModelDto
                {
                    CustomerViolationGetDtos = customerViolationGetDto,
                    CustomerViolationRegisterDto = customerViolationRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", CustomerViolationModelDto);

            }
        }


        [Authorize("Permissions.CustomerViolationEdit")]
        public async Task<IActionResult> GetData(Guid Id)
        {
            var customerViolationById = await _customerViolationRepo.GetByIdAsync(Id);
            if (customerViolationById == null)
                return NotFound();
            var customerViolationRegisterDto = _mapper.Map<CustomerViolationRegisterDto>(customerViolationById);

            customerViolationRegisterDto.Cars = await _carRepo.GetAllAsync();
            customerViolationRegisterDto.Customers = await _customerRepo.GetAllAsync();
            customerViolationRegisterDto.Stocks = await _stockRepo.GetAllAsync();

            return PartialView("_PartialCustomerViolation", customerViolationRegisterDto);
        }


        //For Movement And Account 
        [Authorize("Permissions.CustomerViolationEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var customerViolationById = await _customerViolationRepo.GetByIdAsync(id);
            if (customerViolationById == null)
                return NotFound();
            var CustomerViolationData = await _customerViolationRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CustomerViolationPhotos);
            var customerViolationGetDto = _mapper.Map<List<CustomerViolationGetDto>>(CustomerViolationData);
            var customerViolationRegisterDto = _mapper.Map<CustomerViolationRegisterDto>(customerViolationById);

            customerViolationRegisterDto.Cars = await _carRepo.GetAllAsync();
            customerViolationRegisterDto.Customers = await _customerRepo.GetAllAsync();
            customerViolationRegisterDto.Stocks = await _stockRepo.GetAllAsync();
            var CustomerViolationModelDto = new CustomerViolationModelDto
            {
                CustomerViolationGetDtos = customerViolationGetDto,
                CustomerViolationRegisterDto = customerViolationRegisterDto
            };
            return View("Index", CustomerViolationModelDto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerViolationEdit")]
        public async Task<IActionResult> Edit(CustomerViolationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerViolationById = await _customerViolationRepo.GetByIdAsync((Guid)model.Id);
                if (customerViolationById == null)
                    return NotFound();
                var userAdd = customerViolationById.CreatedUser;
                var userAddDate = customerViolationById.CreatedDate;

                var customer = _customerRepo.SingleOrDefaultAsync(c => c.Id == model.CustomerId).Result.name;
                var StockMovementById = await _stockmovementRepo.SingleOrDefaultAsync(s => s.MovementId == customerViolationById.Id && s.MovementType == StockMovementType.CustomerViolation);

                StockMovementById.Date = (DateTime)model.Date;
                StockMovementById.InValue = (double)model.Payment;
                StockMovementById.OutValue = 0;
                StockMovementById.Notes = model.Notes;
                StockMovementById.StockId = model.StockId;
                StockMovementById.Comment = " دفعة من مخالفة العميل" + " " + customer;
                _stockmovementRepo.Update(StockMovementById);

                var CustomerAccountById = await _customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == customerViolationById.Id && c.AccountType == RentAccountType.Violation);

                CustomerAccountById.Date = (DateTime)model.Date;
                CustomerAccountById.Dept = (double)model.Value;
                CustomerAccountById.Borrower = (double)model.Payment;
                CustomerAccountById.Notes = model.Notes;
                CustomerAccountById.Explain = " دفعة من مخالفة العميل" + " " + customer;
                CustomerAccountById.CustomerId = model.CustomerId;
                _customerAccountRepo.Update(CustomerAccountById);

                if (model.ViolationPhotoFile != null)
                {
                    var customerViolationDb = _mapper.Map(model, customerViolationById);
                    customerViolationDb.CreatedDate = userAddDate;
                    customerViolationDb.CreatedUser = userAdd;
                    customerViolationDb.LastEditDate = DateTime.Now;
                    customerViolationDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _customerViolationRepo.Update(customerViolationDb);
                    string upload = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerViolation/" + customerViolationDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.ViolationPhotoFile)
                    {
                        string fileName = Guid.NewGuid().ToString() + "." + Photo.FileName.Split(".")[1].ToString();
                        string fullPath = Path.Combine(upload, fileName);

                        string fileExtention = Photo.ContentType;
                        var fileLenght = Photo.Length;
                        if (fileExtention == "image/png" || fileExtention == "image/jpeg" || fileExtention == "image/x-png" || fileExtention == "image/jpg")
                        {
                            if (fileLenght >= 3145728)
                            {
                                Bitmap bmpPostedImage = new Bitmap(Photo.OpenReadStream());
                                Image objImage = ResizeImage.ScaleImage(bmpPostedImage, 1000);
                                // Saving image in jpeg format
                                objImage.Save(fullPath, ImageFormat.Jpeg);
                            }
                            else
                            {
                                Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                            }
                        }
                        else
                        {
                            Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                        }
                        var customerViolationPhoto = new CustomerViolationPhoto
                        {
                            CustomerViolationId = customerViolationDb.Id,
                            PhotoUrl = fileName
                        };
                        _customerViolationPhotoRepo.Add(customerViolationPhoto);
                        await _customerViolationPhotoRepo.SaveAllAsync();
                    }
                }
                else
                {
                    model.CustomerViolationPhotos = customerViolationById.CustomerViolationPhotos;
                    var customerViolationDb = _mapper.Map(model, customerViolationById);
                    customerViolationDb.CreatedDate = userAddDate;
                    customerViolationDb.CreatedUser = userAdd;
                    customerViolationDb.LastEditDate = DateTime.Now;
                    customerViolationDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _customerViolationRepo.Update(customerViolationDb);
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                await _customerViolationRepo.SaveAllAsync();
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerViolationData = await _customerViolationRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CustomerViolationPhotos);
                var customerViolationGetDto = _mapper.Map<List<CustomerViolationGetDto>>(CustomerViolationData);
                var customerViolationRegisterDto = _mapper.Map<CustomerViolationRegisterDto>(model);

                customerViolationRegisterDto.Cars = await _carRepo.GetAllAsync();
                customerViolationRegisterDto.Customers = await _customerRepo.GetAllAsync();
                customerViolationRegisterDto.Stocks = await _stockRepo.GetAllAsync();

                var CustomerViolationModelDto = new CustomerViolationModelDto
                {
                    CustomerViolationGetDtos = customerViolationGetDto,
                    CustomerViolationRegisterDto = customerViolationRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", CustomerViolationModelDto);

            }

        }

        [Authorize("Permissions.CustomerViolationDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var customerViolationById = await _customerViolationRepo.GetByIdAsync(Id);
            if (customerViolationById == null)
                return NotFound();
            var StockMovementById = await _stockmovementRepo.SingleOrDefaultAsync(s => s.MovementId == customerViolationById.Id && s.MovementType == StockMovementType.CustomerViolation);
            var CustomerAccountById = await _customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == customerViolationById.Id && c.AccountType == RentAccountType.Violation);
            _stockmovementRepo.Delete(StockMovementById);
            _customerAccountRepo.Delete(CustomerAccountById);
            var photos = await _customerViolationPhotoRepo.GetAllAsync(p => p.CustomerViolationId == Id);
            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    await DeletePhoto(photo.Id);
                }
            }
            _customerViolationRepo.Delete(customerViolationById);
            await _customerViolationRepo.SaveAllAsync();
            return Ok();
        }


        //Get CustomerViolation Photos
        [AllowAnonymous]
        public IActionResult GetCustomerViolationPhotos(Guid id)
        {
            var CustomerViolationPhotos = _customerViolationPhotoRepo.GetAllAsync(p => p.CustomerViolationId == id, p => p.CustomerViolation).Result;
            string FilePath = "/Images/CustomerViolation/" + id.ToString();

            return Json(CustomerViolationPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.CustomerViolation.ViolationNumber }));
        }

        //Delete Photo
        [AllowAnonymous]
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await _customerViolationPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerViolation/" + Photo.CustomerViolationId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            _customerViolationPhotoRepo.Delete(Photo);
            await _customerViolationPhotoRepo.SaveAllAsync();

            return Ok();
        }


        [AllowAnonymous]
        public async Task<IActionResult> GetCustomer(Guid id,DateTime date)
        {
            var lastCustomerRent = _customerRentRepo.GetAllAsync(c => c.CarId == id).Result.Where(c => c.EndDate.Date >= date.Date && c.StartDate.Date <= date.Date).LastOrDefault();
            var customer =await _customerRepo.SingleOrDefaultAsync(c => c.Id == lastCustomerRent.CustomerId);
            return Json(new { id= customer .Id});
        }
    }
}
