using ContactManager.Core.Domain.IdentityEntities;
using ContactManager.Core.DTO;
using ContactManager.Core.Enums;
using CRUDExample.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ContactManager.UI.Controllers
{
    [Route("[controller]/[action]")]
  
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;


        private readonly RoleManager<ApplicationRole> _roleManager;


        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManger
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManger;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateEmail(string Email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }


        [HttpGet]
        [Authorize("NotAuthenticatedUser")]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticatedUser")]

        public async Task<IActionResult> Login(LoginDTO loginDTO, string? ReturnUrl = "")
        {

            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
                return View(loginDTO);
            }


            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, false, false);

            if (result.Succeeded)
            {

                ApplicationUser User_use = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (User_use is not  null)
                {
                    if (await _userManager.IsInRoleAsync(User_use, UserTypeOptions.Admin.ToString()))
                    {
                      return  RedirectToAction("Index", "Home", new { area = "Admin" });
                    }
                }

                if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                {
                 return   LocalRedirect(ReturnUrl);
                }

                return RedirectToAction(nameof(PersonsController.Index), "Persons");
            }

            ModelState.AddModelError("LogIn", "Error in Log In");
            return View(loginDTO);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SingOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(PersonsController.Index), "Persons");
        }

        [HttpGet]
        [Authorize("NotAuthenticatedUser")]
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthenticatedUser")]
       // [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {


            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
                return View(registerDTO);
            }


            ApplicationUser applicationUser = new ApplicationUser()
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                PersonName = registerDTO.PersonName
            };


            IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, registerDTO.Password);

            if (identityResult.Succeeded)
            {

                await _signInManager.SignInAsync(applicationUser, false, null);



                //Check if user select as Admin Role
                if (registerDTO.UserType == UserTypeOptions.Admin)
                {
                    //Check is Admin Role is Exit In AspNeyRole Tbale
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        //Creaaet Objet g apptical Role to Create New Role Admin 
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.Admin.ToString() };



                        //Using Role Manage we can Crate New Role y Paaing Object Of Application Role 
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    //Assing Role For This User , sinu User Manger we can Assing Role to particular user
                    identityResult = await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.Admin.ToString());

                }
                else if (registerDTO.UserType == UserTypeOptions.User)
                {
                    //Check is user  Role is Exit In AspNeyRole Tbale
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        //Creaaet Objet g apptical Role to Create New Role user 
                        ApplicationRole applicationRole = new ApplicationRole() { Name = UserTypeOptions.User.ToString() };



                        //Using Role Manage we can Crate New Role y Paaing Object Of Application Role 
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    //Assing Role For This User , sinu User Manger we can Assing Role to particular user
                    identityResult = await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.User.ToString());

                }
                else
                {

                }

                return RedirectToAction("Index", "Persons");
            }
            else
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError("Regiteresion", error.Description);
                }
                ViewBag.Errors = ModelState.Values.SelectMany(x => x.Errors).Select(e => e.ErrorMessage);
                return View(registerDTO);
            }


        }
    }
}
