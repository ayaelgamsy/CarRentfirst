using AutoMapper;
using Core.Dtos;
using Core.Dtos.UserDto;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SiteFront.Areas.Managment.Controllers
{
    [Area("Managment")]
    //[AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IToastNotification _toastNotification;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<User> _UserRepo;
        private readonly ILogger<RegisterModel> _logger;
        private readonly  IRepository<UserRole> _UserRoleRepo;


        public List<string> RolesSelect { get; set; } = new List<string>();
        public List<Role> roles { get; set; }
        public List<AuthenticationScheme> ExternalLogins { get; private set; }



        public UsersController(
            IRepository<User> UserRepo,
            UserManager<User> userManager,
            RoleManager<Role> RoleManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IMapper mapper, 
            IRepository<UserRole> UserRoleRepo, 
            IToastNotification toastNotification)
        {
            _UserRepo = UserRepo;
            _mapper = mapper;
            _UserRoleRepo = UserRoleRepo;
            _toastNotification = toastNotification;

            _roleManager = RoleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

          
        }

        [Authorize("Permissions.UsersIndex")]
         [HttpGet]
        public async Task<IActionResult> Index()
        {
            var UsersDb = await _userManager.Users.Select(User => new UserGetDto { Id = User.Id, Name = User.Name, Email = User.Email, PhoneNumber = User.PhoneNumber, Roles = _userManager.GetRolesAsync(User).Result }).ToListAsync();
            return View(UsersDb);
        }


        [Authorize("Permissions.UsersCreate")]

        //[Authorize("UserCreate")]
        public async Task<IActionResult> Create()
        {
            var Roles = await _roleManager.Roles.ToListAsync();

            UserRegisterDto userRegisterDto = new UserRegisterDto();
           
            userRegisterDto.Roles = Roles.Select(n => new CommonDto
            {
                Id = n.Id,
                Name = n.Name,
                Isselected = false

            }).ToList();
            return View(userRegisterDto);
        }




        [Authorize("Permissions.UsersCreate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRegisterDto model, string returnUrl = null)
        {
           
            if (ModelState.IsValid)
            {
                User user = _mapper.Map<User>(model);
                user.UserName = new MailAddress(model.Email).User;

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                      user = await _userManager.FindByIdAsync(user.Id.ToString());
                 

                    foreach (var item in model.Roles.Where(n => n.Isselected))
                    {
                        UserRole userRole = new UserRole()
                        {
                            RoleId = item.Id,
                            UserId = user.Id
                        };

                        _UserRoleRepo.Add(userRole);
                        await _UserRoleRepo.SaveAllAsync();
                    }
                    
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return RedirectToAction("Index");
        }


        // GET: Managment/Users/Edit/5
        [Authorize("Permissions.UsersEdit")]

        public async Task<IActionResult> Edit(Guid? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var user = await _UserRepo.GetByIdAsync((Guid)Id);

            if (user == null)
            {
                return NotFound();
            }

            RolesSelect = (List<string>)await _userManager.GetRolesAsync(user);

            var AllRolesUser = await _roleManager.Roles.ToListAsync();

            var rolesUser = _mapper.Map<List<CommonDto>>(AllRolesUser);


            setSelected(ref rolesUser);

            UserEditDto viemodel = _mapper.Map<UserEditDto>(user);
            viemodel.Roles = rolesUser;

            return View(viemodel);
        }

        public void setSelected(ref List<CommonDto> roles)
        {
            roles.ForEach(e =>
            {
                if (RolesSelect.Contains(e.Name))
                {
                    e.Isselected = true;
                }
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.UsersEdit")]
        public async Task<IActionResult> Edit(UserEditDto model)
        {

            if (ModelState.IsValid)
            {

                var UserDb = await _UserRepo.GetByIdAsync((Guid)model.Id);

                if (UserDb == null)

                    return NotFound();

                var userDBMapped = _mapper.Map(model, UserDb);

                _UserRepo.Update(userDBMapped);

                List<UserRole> UserRules = (List<UserRole>)await _UserRoleRepo.GetAllAsync(n => n.UserId == model.Id);

                _UserRoleRepo.DeletelistRange(UserRules);


                foreach (var item in model.Roles.Where(n => n.Isselected))
                {
                    UserRole userRole = new UserRole()
                    {
                        RoleId = item.Id,
                        UserId = model.Id
                    };

                    _UserRoleRepo.Add(userRole);

                }

               await _UserRoleRepo.SaveAllAsync();

                _toastNotification.AddSuccessToastMessage("تم التعديل");
            }
            return RedirectToAction("Index");
        }


        [Authorize("Permissions.UsersDelete")]

        // GET: Managment/Users/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _UserRepo.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Managment/Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize("Permissions.UsersDelete")]

        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _UserRepo.GetByIdAsync(id);
            _UserRepo.Delete(user);
            await _UserRepo.SaveAllAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _UserRepo.GetByIdAsync(id) == null ? false : true;
            
        }
    }
}
