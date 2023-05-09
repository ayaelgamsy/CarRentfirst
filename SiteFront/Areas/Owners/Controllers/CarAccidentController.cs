using AutoMapper;
using Core.Common.enums;
using Core.Dtos.CarAccidentDto;
using Core.Dtos.ExpenseDto;
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

namespace SiteFront.Areas.Owners.Controllers
{
    [Area("Owners")]
    //  [AllowAnonymous]
    public class CarAccidentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly IRepository<CarAccident> carAccidentRepo;
        private readonly IRepository<CarAccidentPhoto> carAccidentPhotoRepo;
        private readonly IRepository<CarAccidentVideo> carAccidentVideoRepo;
        private readonly IRepository<Car> carRepo;
        private readonly IRepository<Customer> customerRepo;
        private readonly IRepository<Stock> stockRepo;
        private readonly IRepository<StockMovement> stockMovementRepo;
        private readonly IRepository<CustomerAccount> customerAccountRepo;
        private readonly IWebHostEnvironment _hosting;
        private readonly UserManager<User> _userManager;

        public CarAccidentController(IMapper mapper,
            IToastNotification toastNotification,
            IRepository<CarAccident> CarAccidentRepo,
            IRepository<CarAccidentPhoto> CarAccidentPhotoRepo,
            IRepository<CarAccidentVideo> CarAccidentVideoRepo,
            IRepository<Car> CarRepo,
            IRepository<Customer> CustomerRepo,
            IRepository<Stock> StockRepo,
            IRepository<StockMovement> StockMovementRepo,
            IRepository<CustomerAccount> CustomerAccountRepo,
            IWebHostEnvironment hosting,
            UserManager<User> userManager )
        {
            _mapper = mapper;
            _toastNotification = toastNotification;
            carAccidentRepo = CarAccidentRepo;
            carAccidentPhotoRepo = CarAccidentPhotoRepo;
            carAccidentVideoRepo = CarAccidentVideoRepo;
            carRepo = CarRepo;
            customerRepo = CustomerRepo;
            stockRepo = StockRepo;
            stockMovementRepo = StockMovementRepo;
            customerAccountRepo = CustomerAccountRepo;
            _hosting = hosting;
            _userManager = userManager;
        }

        private Task<User> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);


        [Authorize("Permissions.CarAccidentIndex")]
        public async Task<IActionResult> Index()
        {
            var carAccidentData = await carAccidentRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CarAccidentPhotos, c => c.CarAccidentVideos);
            var carAccidentGetDto = _mapper.Map<List<CarAccidentGetDto>>(carAccidentData);
            var cars = await carRepo.GetAllAsync();
            var customers = await customerRepo.GetAllAsync();
            var stockes = await stockRepo.GetAllAsync();
            var carAccidentRegisterDto = new CarAccidentRegisterDto
            {
                Cars = _mapper.Map<List<DrpDto>>(cars),
                Customers = _mapper.Map<List<DrpDto>>(customers),
                Stocks = _mapper.Map<List<DrpDto>>(stockes)
            };
            var carAccisentModelDto = new CarAccidentModelDto
            {
                CarAccidentGetDtos = carAccidentGetDto,
                CarAccidentRegisterDto = carAccidentRegisterDto
            };

            return View(carAccisentModelDto);
        }

        [Authorize("Permissions.CarAccidentCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarAccidentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarAccidentDbMapped = _mapper.Map<CarAccident>(model);
                CarAccidentDbMapped.CreatedDate = DateTime.Now;
                CarAccidentDbMapped.CreatedUser = (GetCurrentUserAsync().Result).Id.ToString();
                carAccidentRepo.Add(CarAccidentDbMapped);
                var customer = customerRepo.SingleOrDefaultAsync(c => c.Id == CarAccidentDbMapped.CustomerId).Result.name;
                var stockMovement = new StockMovement
                {
                    MovementId = CarAccidentDbMapped.Id,
                    MovementType=StockMovementType.Accident,
                    Date = CarAccidentDbMapped.Date,
                    InValue = CarAccidentDbMapped.Payment,
                    OutValue = 0,
                    Notes = CarAccidentDbMapped.Notes,
                    StockId = CarAccidentDbMapped.StockId,
                    Comment = " دفعة من حادثة العميل"+" " + customer,
                };
                stockMovementRepo.Add(stockMovement);
                var customerAccount = new CustomerAccount
                {
                    MovementId = CarAccidentDbMapped.Id,
                    AccountType=RentAccountType.Accident,
                    Date = CarAccidentDbMapped.Date,
                    Dept = CarAccidentDbMapped.Price,
                    Borrower = CarAccidentDbMapped.Payment,
                    Notes = CarAccidentDbMapped.Notes,
                    Explain = " دفعة من حادثة العميل" +" "+ customer,
                    CustomerId = CarAccidentDbMapped.CustomerId
                };
                customerAccountRepo.Add(customerAccount);
                await carAccidentRepo.SaveAllAsync();

                //For Photos
                if (model.CarAccidentPhotosFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Images/CarAccidents/Photos/" + CarAccidentDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile item in model.CarAccidentPhotosFile)
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

                        var carAccidentPhoto = new CarAccidentPhoto
                        {
                            PhotoUrl = fileName,
                            CarAccidentId = CarAccidentDbMapped.Id
                        };
                        carAccidentPhotoRepo.Add(carAccidentPhoto);
                        await carAccidentPhotoRepo.SaveAllAsync();
                    }
                }
                //For Videos
                if (model.CarAccidentVideosFile != null)
                {
                    var upload = Path.Combine(_hosting.WebRootPath, "Videos/CarAccidents/" + CarAccidentDbMapped.Id.ToString());
                    if (!Directory.Exists(upload))
                        Directory.CreateDirectory(upload);
                    foreach (IFormFile item in model.CarAccidentVideosFile)
                    {
                        var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                        var FullPath = Path.Combine(upload, fileName);
                        item.CopyTo(new FileStream(FullPath, FileMode.Create));

                        var carAccidentvideo = new CarAccidentVideo
                        {
                            VideoUrl = fileName,
                            CarAccidentId = CarAccidentDbMapped.Id
                        };
                        carAccidentVideoRepo.Add(carAccidentvideo);
                        await carAccidentVideoRepo.SaveAllAsync();
                    }
                }
                _toastNotification.AddSuccessToastMessage("تم الاضافة");
                return RedirectToAction("Index");
            }
            else { 
            var carAccidentData = await carAccidentRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CarAccidentPhotos, c => c.CarAccidentVideos);
            var carAccidentGetDto = _mapper.Map<List<CarAccidentGetDto>>(carAccidentData);
            var cars = await carRepo.GetAllAsync();
            var customers = await customerRepo.GetAllAsync();
            var stockes = await stockRepo.GetAllAsync();
            var carAccidentRegisterDto = _mapper.Map<CarAccidentRegisterDto>(model);
            carAccidentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(cars);
            carAccidentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
            carAccidentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(stockes);
            var carAccisentModelDto = new CarAccidentModelDto
            {
                CarAccidentGetDtos = carAccidentGetDto,
                CarAccidentRegisterDto = carAccidentRegisterDto
            };
            _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
            return View("Index",carAccisentModelDto);
            }
        }


        [Authorize("Permissions.CarAccidentEdit")]
        public async Task<IActionResult> GetData(Guid id)
        {
            var CarAccidentById = await carAccidentRepo.GetByIdAsync(id);
            var carAccidentRegisterDto = _mapper.Map<CarAccidentRegisterDto>(CarAccidentById);
            var cars = await carRepo.GetAllAsync();
            var customers = await customerRepo.GetAllAsync();
            var stockes = await stockRepo.GetAllAsync();
            carAccidentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(cars);
            carAccidentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
            carAccidentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(stockes);

            return PartialView("_PartialCarAccident", carAccidentRegisterDto);
        }

        //For Movement And Account 
        [Authorize("Permissions.CarAccidentEdit")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var CarAccidentById = await carAccidentRepo.GetByIdAsync(id);
            var carAccidentData = await carAccidentRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CarAccidentPhotos, c => c.CarAccidentVideos);
            var carAccidentGetDto = _mapper.Map<List<CarAccidentGetDto>>(carAccidentData);
            var carAccidentRegisterDto = _mapper.Map<CarAccidentRegisterDto>(CarAccidentById);
            var cars = await carRepo.GetAllAsync();
            var customers = await customerRepo.GetAllAsync();
            var stockes = await stockRepo.GetAllAsync();
            carAccidentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(cars);
            carAccidentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
            carAccidentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(stockes);
            var carAccisentModelDto = new CarAccidentModelDto
            {
                CarAccidentGetDtos = carAccidentGetDto,
                CarAccidentRegisterDto = carAccidentRegisterDto
            };
            return View("Index", carAccisentModelDto);
        }

        [Authorize("Permissions.CarAccidentEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CarAccidentRegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var CarAccidentById = await carAccidentRepo.GetByIdAsync((Guid)model.Id);
                
                var userAdd = CarAccidentById.CreatedUser;
                var userAddDate = CarAccidentById.CreatedDate;

                var customer = customerRepo.SingleOrDefaultAsync(c => c.Id == CarAccidentById.CustomerId).Result.name;
                var StockMovementById = await stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == CarAccidentById.Id && s.MovementType==StockMovementType.Accident);
                
                StockMovementById.Date =(DateTime)model.Date;
                StockMovementById.InValue = (double)model.Payment;
                StockMovementById.OutValue = 0;
                StockMovementById.Notes = model.Notes;
                StockMovementById.StockId = model.StockId;
                StockMovementById.Comment = " دفعة من حادثة العميل"+" " + customer;
                stockMovementRepo.Update(StockMovementById);

                var CustomerAccountById = await customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == CarAccidentById.Id && c.AccountType==RentAccountType.Accident);
                
                CustomerAccountById.Date =(DateTime)model.Date;
                CustomerAccountById.Dept =(double)model.Price;
                CustomerAccountById.Borrower = (double)model.Payment;
                CustomerAccountById.Notes = model.Notes;
                CustomerAccountById.Explain = " دفعة من حادثة العميل"+" " + customer;
                CustomerAccountById.CustomerId = model.CustomerId;
                customerAccountRepo.Update(CustomerAccountById);

                if (model.CarAccidentPhotosFile != null || model.CarAccidentVideosFile != null)
                {
                    var CarAccidentDbMapped = _mapper.Map(model, CarAccidentById);
                    CarAccidentDbMapped.CreatedDate = userAddDate;
                    CarAccidentDbMapped.CreatedUser = userAdd;
                    CarAccidentDbMapped.LastEditDate = DateTime.Now;
                    CarAccidentDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    carAccidentRepo.Update(CarAccidentDbMapped);
                    await carAccidentRepo.SaveAllAsync();
                    //For Photos
                    if (model.CarAccidentPhotosFile != null)
                    {
                        var upload = Path.Combine(_hosting.WebRootPath, "Images/CarAccidents/Photos/" + CarAccidentDbMapped.Id.ToString());
                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);
                        foreach (IFormFile item in model.CarAccidentPhotosFile)
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
                            var carAccidentPhoto = new CarAccidentPhoto
                            {
                                PhotoUrl = fileName,
                                CarAccidentId = CarAccidentById.Id
                            };
                            carAccidentPhotoRepo.Add(carAccidentPhoto);
                            await carAccidentPhotoRepo.SaveAllAsync();
                        }
                    }
                    //For Videos
                    if (model.CarAccidentVideosFile != null)
                    {

                        var upload = Path.Combine(_hosting.WebRootPath, "Videos/CarAccidents/" + CarAccidentDbMapped.Id.ToString());
                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);
                        //AddNewPath
                        foreach (IFormFile item in model.CarAccidentVideosFile)
                        {
                            var fileName = Guid.NewGuid().ToString() + "." + item.FileName.Split(".")[1].ToString();
                            var FullPath = Path.Combine(upload, fileName);
                            item.CopyTo(new FileStream(FullPath, FileMode.Create));
                            var carAccidentVideo = new CarAccidentVideo
                            {
                                VideoUrl = fileName,
                                CarAccidentId = CarAccidentById.Id
                            };
                            carAccidentVideoRepo.Add(carAccidentVideo);
                            await carAccidentVideoRepo.SaveAllAsync();
                        }
                    }
                }
                else
                {
                    model.CarAccidentPhotos = CarAccidentById.CarAccidentPhotos;
                    model.CarAccidentVideos = CarAccidentById.CarAccidentVideos;
                    var CarAccidentDbMapped = _mapper.Map(model, CarAccidentById);
                    CarAccidentDbMapped.CreatedDate = userAddDate;
                    CarAccidentDbMapped.CreatedUser = userAdd;
                    CarAccidentDbMapped.LastEditDate = DateTime.Now;
                    CarAccidentDbMapped.LastEditUser = (await GetCurrentUserAsync()).Id.ToString();
                    carAccidentRepo.Update(CarAccidentDbMapped);
                    await carAccidentRepo.SaveAllAsync();
                }
                _toastNotification.AddSuccessToastMessage("تم التعديل");
                return RedirectToAction("Index");
            }
            else
            {
                var carAccidentData = await carAccidentRepo.GetAllAsync(c => c.Car, c => c.Customer, c => c.Stock, c => c.CarAccidentPhotos, c => c.CarAccidentVideos);
                var carAccidentGetDto = _mapper.Map<List<CarAccidentGetDto>>(carAccidentData);
                var cars = await carRepo.GetAllAsync();
                var customers = await customerRepo.GetAllAsync();
                var stockes = await stockRepo.GetAllAsync();
                var carAccidentRegisterDto = _mapper.Map<CarAccidentRegisterDto>(model);
                carAccidentRegisterDto.Cars = _mapper.Map<List<DrpDto>>(cars);
                carAccidentRegisterDto.Customers = _mapper.Map<List<DrpDto>>(customers);
                carAccidentRegisterDto.Stocks = _mapper.Map<List<DrpDto>>(stockes);
                var carAccisentModelDto = new CarAccidentModelDto
                {
                    CarAccidentGetDtos = carAccidentGetDto,
                    CarAccidentRegisterDto = carAccidentRegisterDto
                };
                _toastNotification.AddErrorToastMessage("بيانات غير صحيحة !! ");
                return View("Index", carAccisentModelDto);
            }
        }

        [Authorize("Permissions.CarAccidentDelete")]

        public async Task<IActionResult> Delete(Guid id)
        {
            var CarAccidentById = await carAccidentRepo.GetByIdAsync(id);
            var oldUrlPhoto = await carAccidentPhotoRepo.GetAllAsync(e => e.CarAccidentId == id);
            var oldUrlVideo = await carAccidentVideoRepo.GetAllAsync(e => e.CarAccidentId == id);
            if (CarAccidentById == null)
                return NotFound();
            var CustomerAccountById = await customerAccountRepo.SingleOrDefaultAsync(c => c.MovementId == id && c.AccountType == RentAccountType.Accident);
            var StockMovementById = await stockMovementRepo.SingleOrDefaultAsync(s => s.MovementId == id && s.MovementType == StockMovementType.Accident);
            customerAccountRepo.Delete(CustomerAccountById);
            stockMovementRepo.Delete(StockMovementById);
            carAccidentRepo.Delete(CarAccidentById);
            //DeleteOldPath For Photo   
            var uploadPhoto = Path.Combine(_hosting.WebRootPath + "/Images/CarAccidents/Photos/" + CarAccidentById.Id.ToString());
            foreach (var urlPhoto in oldUrlPhoto)
            {
                var oldFileName = urlPhoto.PhotoUrl;
                var oldFullPath = Path.Combine(uploadPhoto, oldFileName);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(oldFullPath);
            }
            //DeleteOldPath For Video   
            var uploadVideo = Path.Combine(_hosting.WebRootPath + "/Videos/CarAccidents/" + CarAccidentById.Id.ToString());
            foreach (var urlVideo in oldUrlVideo)
            {
                var oldFileName = urlVideo.VideoUrl;
                var oldFullPath = Path.Combine(uploadVideo, oldFileName);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(oldFullPath);
            }
            await carAccidentRepo.SaveAllAsync();   
            return Ok();

        }


        //Get CarAccident Photos
        public IActionResult GetCarAccidentPhotos(Guid id)
        {
            var CarAccidentPhotos = carAccidentPhotoRepo.GetAllAsync(p => p.CarAccidentId == id, p => p.CarAccident).Result;
            string FilePath = "/Images/CarAccidents/Photos/" + id.ToString();

            return Json(CarAccidentPhotos.Select(p => new { image = FilePath + "/" + p.PhotoUrl, id = p.Id, name = p.CarAccident.Date }));
        }

        //Delete Photo
        public async Task<IActionResult> DeletePhoto(Guid id)
        {
            var Photo = await carAccidentPhotoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Images/CarAccidents/Photos/" + Photo.CarAccidentId.ToString());
            var FileName = Photo.PhotoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            carAccidentPhotoRepo.Delete(Photo);
            await carAccidentPhotoRepo.SaveAllAsync();

            return Ok();
        }

        //Get Car Videos
        public IActionResult GetCarAccidentVideos(Guid id)
        {
            var CarAccidentVideos = carAccidentVideoRepo.GetAllAsync(p => p.CarAccidentId == id, p => p.CarAccident).Result;
            string FilePath = "/Videos/CarAccidents/" + id.ToString();

            return Json(CarAccidentVideos.Select(p => new { video = FilePath + "/" + p.VideoUrl, id = p.Id, name = p.CarAccident.Date }));
        }

        //Delete Video
        public async Task<IActionResult> DeleteVideo(Guid id)
        {
            var video = await carAccidentVideoRepo.GetByIdAsync(id);
            ////DeleteOldPath   
            string FilePath = Path.GetFullPath("wwwroot/Videos/CarAccidents/" + video.CarAccident.ToString());
            var FileName = video.VideoUrl;
            var FullPath = Path.Combine(FilePath, FileName);
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            System.IO.File.Delete(FullPath);

            carAccidentVideoRepo.Delete(video);
            await carAccidentVideoRepo.SaveAllAsync();

            return Ok();
        }

    }
}
