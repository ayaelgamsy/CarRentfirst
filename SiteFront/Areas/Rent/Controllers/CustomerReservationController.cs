using AutoMapper;
using Core.Dtos.CustomerReservationDto;
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
    //[AllowAnonymous]
    public class CustomerReservationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IWebHostEnvironment _hosting;
        private readonly IRepository<CustomerReservation> _customerReservationRepo;
        private readonly IRepository<Car> _carRepo;
        private readonly IRepository<Marketer> _marketerRepo;
        private readonly IRepository<CustomerReservationPhoto> _customerReservationPhotoRepo;
        private readonly UserManager<User> _userManager;

        public CustomerReservationController(IMapper mapper,
            IToastNotification toastNotification,
            IWebHostEnvironment hosting,
            IRepository<CustomerReservation> customerReservationRepo,
            IRepository<Car> carRepo,
            IRepository<Marketer> marketerRepo,
            IRepository<CustomerReservationPhoto> customerReservationPhotoRepo,
            UserManager<User> userManager)
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            _hosting = hosting;
            _customerReservationRepo = customerReservationRepo;
            _carRepo = carRepo;
            _marketerRepo = marketerRepo;
            _customerReservationPhotoRepo = customerReservationPhotoRepo;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        [Authorize("Permissions.CustomerReservationIndex")]
        public async Task<IActionResult> Index()
        {
            var customerReservationData =await _customerReservationRepo.GetAllAsync(c => c.Car, c => c.Marketer, c => c.CustomerReservationPhotos);
            var customerReservationGetDto = _mapper.Map<List<CustomerReservationGetDto>>(customerReservationData);
            var customerReservationRegisterDto = new CustomerReservationRegisterDto
            {
                Cars = await _carRepo.GetAllAsync(),
                Marketers = await _marketerRepo.GetAllAsync()
            };
            var customerReservationModelDto = new CustomerReservationModelDto
            {
                CustomerReservationGetDtos = customerReservationGetDto,
                CustomerReservationRegisterDto = customerReservationRegisterDto
            };
            return View(customerReservationModelDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerReservationCreate")]
        public async Task<IActionResult> Create(CustomerReservationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerReservationDb = _mapper.Map<CustomerReservation>(model);
                customerReservationDb.CreatedDate = DateTime.Now;
                customerReservationDb.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                _customerReservationRepo.Add(customerReservationDb);
               
                if (model.ReservationPhotoFile != null)
                {
                    string upload = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerReservation/" + customerReservationDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.ReservationPhotoFile)
                    {
                        string fileName =Guid.NewGuid().ToString()+"."+ Photo.FileName.Split(".")[1].ToString();
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
                        var customerReservatioPhoto = new CustomerReservationPhoto
                        {
                            CustomerReservationId = customerReservationDb.Id,
                            PhotoUrl = fileName
                        };
                        _customerReservationPhotoRepo.Add(customerReservatioPhoto);
                        await _customerReservationPhotoRepo.SaveAllAsync();
                    }
                }
                await _customerReservationRepo.SaveAllAsync();
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else
            {
                var customerReservationData = await _customerReservationRepo.GetAllAsync(c => c.Car, c => c.Marketer, c => c.CustomerReservationPhotos);
                var customerReservationGetDto = _mapper.Map<List<CustomerReservationGetDto>>(customerReservationData);
                var customerReservationRegisterDto = _mapper.Map<CustomerReservationRegisterDto>(model);

                customerReservationRegisterDto.Cars = await _carRepo.GetAllAsync();
                customerReservationRegisterDto.Marketers = await _marketerRepo.GetAllAsync();

                var customerReservationModelDto = new CustomerReservationModelDto
                {
                    CustomerReservationGetDtos = customerReservationGetDto,
                    CustomerReservationRegisterDto = customerReservationRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index",customerReservationModelDto);

            }
        }

        [Authorize("Permissions.CustomerReservationEdit")]
        public async Task<IActionResult> GetData(Guid Id)
        {
            var customerReservationById = await _customerReservationRepo.GetByIdAsync(Id);
            if (customerReservationById == null)
                return NotFound();
            var customerReservationRegisterDto = _mapper.Map<CustomerReservationRegisterDto>(customerReservationById);

            customerReservationRegisterDto.Cars = await _carRepo.GetAllAsync();
            customerReservationRegisterDto.Marketers = await _marketerRepo.GetAllAsync();
            return PartialView("_PartialCustomerReservation", customerReservationRegisterDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.CustomerReservationEdit")]
        public async Task<IActionResult> Edit(CustomerReservationRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var customerReservationById = await _customerReservationRepo.GetByIdAsync((Guid)model.Id);
                if (customerReservationById == null)
                    return NotFound();
                var userAdd = customerReservationById.CreatedUser;
                var userAddDate = customerReservationById.CreatedDate;

                if (model.ReservationPhotoFile != null)
                {
                    var customerReservationDb = _mapper.Map(model, customerReservationById);
                    customerReservationDb.CreatedDate = userAddDate;
                    customerReservationDb.CreatedUser = userAdd;
                    customerReservationDb.LastEditDate = DateTime.Now;
                    customerReservationDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _customerReservationRepo.Update(customerReservationDb);
                    string upload = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerReservation/" + customerReservationDb.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile Photo in model.ReservationPhotoFile)
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
                        var customerReservatioPhoto = new CustomerReservationPhoto
                        {
                            CustomerReservationId = customerReservationDb.Id,
                            PhotoUrl = fileName
                        };
                        _customerReservationPhotoRepo.Add(customerReservatioPhoto);
                        await _customerReservationPhotoRepo.SaveAllAsync();
                    }
                }
                else
                {
                    model.CustomerReservationPhotos = customerReservationById.CustomerReservationPhotos;
                    var customerReservationDb = _mapper.Map(model, customerReservationById);
                    customerReservationDb.CreatedDate = userAddDate;
                    customerReservationDb.CreatedUser = userAdd;
                    customerReservationDb.LastEditDate = DateTime.Now;
                    customerReservationDb.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    _customerReservationRepo.Update(customerReservationDb);
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                await _customerReservationRepo.SaveAllAsync();
                return RedirectToAction("Index");
            }
            else
            {
                var customerReservationData = await _customerReservationRepo.GetAllAsync(c => c.Car, c => c.Marketer, c => c.CustomerReservationPhotos);
                var customerReservationGetDto = _mapper.Map<List<CustomerReservationGetDto>>(customerReservationData);
                var customerReservationRegisterDto = _mapper.Map<CustomerReservationRegisterDto>(model);

                customerReservationRegisterDto.Cars = await _carRepo.GetAllAsync();
                customerReservationRegisterDto.Marketers = await _marketerRepo.GetAllAsync();

                var customerReservationModelDto = new CustomerReservationModelDto
                {
                    CustomerReservationGetDtos = customerReservationGetDto,
                    CustomerReservationRegisterDto = customerReservationRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة");
                return View("Index", customerReservationModelDto);

            }

        }

        [Authorize("Permissions.CustomerReservationDelete")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var customerReservationById = await _customerReservationRepo.GetByIdAsync(Id);
            if (customerReservationById == null)
                return NotFound();
            var photos = await _customerReservationPhotoRepo.GetAllAsync(p => p.CustomerReservationId == Id);
            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    await DeletePhoto(photo.Id);
                }
            }
            _customerReservationRepo.Delete(customerReservationById);
             await _customerReservationRepo.SaveAllAsync();
            return Ok();
        }

       
        //Get CustomerReservation Photos
        [AllowAnonymous]
        public IActionResult GetCustomerReservationPhotos(Guid id)
        {
            var CustomerReservationPhotos = _customerReservationPhotoRepo.GetAllAsync(p => p.CustomerReservationId == id, p => p.CustomerReservation).Result;
            string FilePath = "/Images/CustomerReservation/" + id.ToString();

            return Json(CustomerReservationPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.CustomerReservation.Customer }));
        }

        [Authorize("Permissions.CustomerReservationDelete")]
        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await _customerReservationPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath(_hosting.WebRootPath + "/Images/CustomerReservation/" + Photo.CustomerReservationId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            _customerReservationPhotoRepo.Delete(Photo);
            await _customerReservationPhotoRepo.SaveAllAsync();

            return Ok();
        }
    }
}
