using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CustomerDto;
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
    public class CustomersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Customer> custRepo;
        private readonly IRepository<Government> goverRepo;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _hosting;
        private readonly IRepository<CustomerPhoto> custPhotoRepo;
        private readonly IRepository<CustomerAccount> _CustomerAccountRepo;
        private readonly IRepository<CustomerRent> _customerRentRepo;
        private readonly IRepository<CustomerPayment> _customerPaymentRepo;
        private readonly IRepository<CarAccident> _carAccidentRepo;
        private readonly IRepository<CustomerLastDept> _customerDeptRepo;
        private readonly UserManager<User> _userManager;

        public CustomersController(IMapper mapper, IRepository<Customer> CustRepo,
            IRepository<Government> GoverRepo,
            IToastNotification toastNotification,
            IWebHostEnvironment hosting,
            IRepository<CustomerPhoto> CustPhotoRepo,
            IRepository<CustomerAccount> CustomerAccountRepo,
            IRepository<CustomerRent> CustomerRentRepo,
            IRepository<CustomerPayment> CustomerPaymentRepo,
            IRepository<CarAccident> CarAccidentRepo,
            IRepository<CustomerLastDept> CustomerDeptRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            custRepo = CustRepo;
            goverRepo = GoverRepo;
            _toastNotification = toastNotification;
            _hosting = hosting;
            custPhotoRepo = CustPhotoRepo;
            _CustomerAccountRepo = CustomerAccountRepo;
            _customerRentRepo = CustomerRentRepo;
            _customerPaymentRepo = CustomerPaymentRepo;
            _carAccidentRepo = CarAccidentRepo;
            _customerDeptRepo = CustomerDeptRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomersIndex")]
        public async Task<IActionResult> Index()
        { 
            var CustomerData = await custRepo.GetAllAsync(c=>c.Government, c => c.CustomerEvaluation, c =>c.CustomerPhotos,c=>c.customerAccounts);
            var customerGetDto = _mapper.Map<List<CustomerGetDto>>(CustomerData);
            var customerRegisterDto = new CustomerRegisterDto
            {
                Governments =await goverRepo.GetAllAsync()
            };
            var CustomerModelDto = new CustomerModelDto
            {
                CustomerGetDtos = customerGetDto,
                CustomerRegisterDto = customerRegisterDto
            };
            return View(CustomerModelDto);   
        }

        [Authorize("Permissions.CustomersIndex")]
        public async Task<IActionResult> CustomerData()
        {
            var CustomerData =  custRepo.GetAllAsync(c => c.Government).Result.Select(c => new CustomerDataDto
            {
                Name=c.name,
                CreatedUser= c.CreatedUser == null ? c.CreatedUser : _userManager.FindByIdAsync(c.CreatedUser).Result.Name,
                CreatedDate=c.CreatedDate,
                IdentNumber=c.IdentNumber,
                LastEditUser= c.LastEditUser == null ? c.LastEditUser : _userManager.FindByIdAsync(c.LastEditUser).Result.Name,
                LastEditDate=c.LastEditDate,
                phone1=c.phone1,
                PassportNumber=c.PassportNumber,
                phone2=c.phone2
            });
            return View(CustomerData);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomersCreate")]
        public async Task<IActionResult> Create(CustomerRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var customers = await custRepo.SingleOrDefaultAsync(c => c.name == model.Name);
                var custFoundCheck=await custRepo.SingleOrDefaultAsync(c=>c.IdentNumber==model.IdentNumber && c.IdentNumber!=null||c.PassportNumber==model.PassportNumber && c.PassportNumber!=null);

                if (custFoundCheck != null) {
                    _toastNotification.AddErrorToastMessage("هذا العميل موجود بالفعل");
                    return RedirectToAction("Index");
                }
                var CustomerDbMapped = _mapper.Map<Customer>(model);
                CustomerDbMapped.CreatedDate = DateTime.Now;
                CustomerDbMapped.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                custRepo.Add(CustomerDbMapped);
                var customerAccount = new CustomerAccount
                {
                    CustomerId = CustomerDbMapped.Id,
                    Dept = CustomerDbMapped.StartAccount,
                    MovementId = CustomerDbMapped.Id,
                    AccountType = RentAccountType.StartAccount,
                    Explain ="الدين البدائي للعميل"+" "+ CustomerDbMapped.name,
                    Date= CustomerDbMapped.Date,
                    Notes = "الدين البدائي للعميل" + " " + CustomerDbMapped.name, 
                };
                _CustomerAccountRepo.Add(customerAccount);
                await custRepo.SaveAllAsync();

                //For Photos
                if (model.CustomerPhotoFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Images/Customers/" + CustomerDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile item in model.CustomerPhotoFile)
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
                        var customerPhoto = new CustomerPhoto
                        {
                            PhotoUrl = fileName,
                            CustomerId = CustomerDbMapped.Id
                        };
                        custPhotoRepo.Add(customerPhoto);
                        await custPhotoRepo.SaveAllAsync();
                    }
                }
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerData = await custRepo.GetAllAsync(c => c.Government, c => c.CustomerEvaluation, c => c.CustomerPhotos, c => c.customerAccounts);
                var customerGetDto = _mapper.Map<List<CustomerGetDto>>(CustomerData);
                var customerRegisterDto = _mapper.Map<CustomerRegisterDto>(model);
                customerRegisterDto.Governments = await goverRepo.GetAllAsync();
                var CustomerModelDto = new CustomerModelDto
                {
                    CustomerGetDtos = customerGetDto,
                    CustomerRegisterDto = customerRegisterDto
                };
                _toastNotification.AddErrorToastMessage(" بيانات غير صحيحة");
                return View("Index",CustomerModelDto);
            }
        }
        
        [Authorize("Permissions.CustomersEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CustomerById = await custRepo.GetByIdAsync(id);
            if (CustomerById == null)
                return NotFound();
            var CustomerRegisterDto = _mapper.Map<CustomerRegisterDto>(CustomerById);
            CustomerRegisterDto.Governments = await goverRepo.GetAllAsync();
            return PartialView("_PartialCustomer", CustomerRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CustomersEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CustomerById = await custRepo.GetByIdAsync(id);
            if (CustomerById == null)
                return NotFound();
            var CustomerData = await custRepo.GetAllAsync(c => c.Government, c => c.CustomerEvaluation, c => c.CustomerPhotos, c => c.customerAccounts);
            var customerGetDto = _mapper.Map<List<CustomerGetDto>>(CustomerData);
            var customerRegisterDto = _mapper.Map<CustomerRegisterDto>(CustomerById);
            customerRegisterDto.Governments = await goverRepo.GetAllAsync();
            var CustomerModelDto = new CustomerModelDto
            {
                CustomerGetDtos = customerGetDto,
                CustomerRegisterDto = customerRegisterDto
            };
            return View("Index", CustomerModelDto);
        }

        [Authorize("Permissions.CustomersEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdentNumber == null && model.PassportNumber == null)
                {
                    _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                    return RedirectToAction("Index");
                }
                var CustomerById = await custRepo.GetByIdAsync((Guid)model.Id);
                var userAdd = CustomerById.CreatedUser;
                var userAddDate = CustomerById.CreatedDate;
                var customerAccountById = await _CustomerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == (Guid)model.Id && c.AccountType == RentAccountType.StartAccount);

                customerAccountById.CustomerId = (Guid)model.Id;
                customerAccountById.Dept = (double)model.StartAccount;
                customerAccountById.MovementId = (Guid)model.Id;
                customerAccountById.AccountType = RentAccountType.StartAccount;
                customerAccountById.Explain = "الدين البدائي للعميل" + " " + model.Name;
                customerAccountById.Date = (DateTime)model.Date;
                customerAccountById.Notes = "الدين البدائي للعميل" + " " + model.Name;
                _CustomerAccountRepo.Update(customerAccountById);
                //For Photos
                if (model.CustomerPhotoFile != null)
                {
                    var CustomerDbMapped = _mapper.Map(model, CustomerById);
                    CustomerDbMapped.CreatedDate = userAddDate;
                    CustomerDbMapped.CreatedUser = userAdd;
                    CustomerDbMapped.LastEditDate = DateTime.Now;
                    CustomerDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    custRepo.Update(CustomerDbMapped);
                    await custRepo.SaveAllAsync();
                    var oldUrlPhoto = await custPhotoRepo.GetAllAsync(p => p.CustomerId== CustomerById.Id);
                    var upload = Path.Combine(_hosting.WebRootPath, "Images/Customers/" + CustomerDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);

                    foreach (IFormFile item in model.CustomerPhotoFile)
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
                        var customerPhoto = new CustomerPhoto
                        {
                            PhotoUrl = fileName,
                            CustomerId = CustomerById.Id
                        };
                        custPhotoRepo.Add(customerPhoto);
                        await custPhotoRepo.SaveAllAsync();
                    }
                }
                else
                {
                    model.CustomerPhotos = CustomerById.CustomerPhotos;
                    var CustomerDbMapped = _mapper.Map(model, CustomerById);
                    CustomerDbMapped.CreatedDate = userAddDate;
                    CustomerDbMapped.CreatedUser = userAdd;
                    CustomerDbMapped.LastEditDate = DateTime.Now;
                    CustomerDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    custRepo.Update(CustomerDbMapped);
                    await custRepo.SaveAllAsync();
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var CustomerData = await custRepo.GetAllAsync(c => c.Government, c => c.CustomerEvaluation, c => c.CustomerPhotos, c => c.customerAccounts);
                var customerGetDto = _mapper.Map<List<CustomerGetDto>>(CustomerData);
                var customerRegisterDto = _mapper.Map<CustomerRegisterDto>(model);
                customerRegisterDto.Governments = await goverRepo.GetAllAsync();
                var CustomerModelDto = new CustomerModelDto
                {
                    CustomerGetDtos = customerGetDto,
                    CustomerRegisterDto = customerRegisterDto
                };
                _toastNotification.AddErrorToastMessage(" بيانات غير صحيحة");
                return View("Index", CustomerModelDto);
            }
        }

        [Authorize("Permissions.CustomersDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var CustomerById = await custRepo.GetByIdAsync(Id);
            if (CustomerById == null)
                return NotFound();
            var CustomerACCount = await _CustomerAccountRepo.GetAllAsync(n => n.CustomerId == Id);
            var customerRent = await _customerRentRepo.GetAllAsync(c => c.CustomerId == Id);
            var CustomerPayment = await _customerPaymentRepo.GetAllAsync(c => c.CustomerId == Id);
            var CustomerDept = await _customerDeptRepo.GetAllAsync(c => c.CustomerId == Id);
            var CarAcciden = await _carAccidentRepo.GetAllAsync(c => c.CustomerId == Id);
            var oldUrlPhoto = await custPhotoRepo.GetAllAsync(p => p.CustomerId == Id);
            if (customerRent.Count() != 0 || CustomerPayment.Count() != 0 || CarAcciden.Count() != 0)
                _toastNotification.AddErrorToastMessage("لا يمكن حذف  هذا العميل !!");
            else
            {
            custRepo.Delete(CustomerById);
            await custRepo.SaveAllAsync();
            //DeleteOldPath
            var upload = Path.Combine(_hosting.WebRootPath, "Images/Customers/" + CustomerById.Id.ToString());
            foreach (var urlPhoto in oldUrlPhoto)
            {
                var oldFileName = urlPhoto.PhotoUrl;
                var oldFullPath = Path.Combine(upload, oldFileName);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(oldFullPath);
                //custPhotoRepo.Delete(urlPhoto);
                //await custPhotoRepo.SaveAllAsync();
            }
            _toastNotification.AddSuccessToastMessage("تم الحذف");
            }
            return RedirectToAction("Index");
        }

        //Get Employee Photos
        public IActionResult GetCustomerPhotos(Guid id)
        {
            var CustomerPhotos = custPhotoRepo.GetAllAsync(p => p.CustomerId == id, p => p.Customer).Result;
            string FilePath = "/Images/Customers/" + id.ToString();

            return Json(CustomerPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.Customer.name }));
        }


        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await custPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/Customers/" + Photo.CustomerId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            custPhotoRepo.Delete(Photo);
            await custPhotoRepo.SaveAllAsync();

            return Ok();
        }
    }
}
