using MySapsApplication.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MySapsApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email, };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ModelState.AddModelError(string.Empty, "Invalid login Attempt");


            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(user.Email, user.Password, user.RememberMe, false);
                if (result.Succeeded)
                {
                    // Check user roles and redirect accordingly
                    var userRoles = await _userManager.GetRolesAsync(await _userManager.FindByEmailAsync(user.Email));
                    if (userRoles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    else if (userRoles.Contains("Manager"))
                    {
                        return RedirectToAction("Record", "Home"); 
                    }
                    else if (userRoles.Contains("Member"))
                    {
                        return RedirectToAction("Index", "Home"); 
                    }
                    else if (userRoles.Contains("CaseAdmin"))
                    {
                        return RedirectToAction("CaseAdmin", "Account");
                    }
                    else
                    {
                        return RedirectToAction("Home", "Home"); 
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Home", "Home");
        }

        public async Task<IActionResult> CaseAdmin()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }



        
        [HttpPost]
        public async Task<IActionResult> Edit(string userId, string oldRole, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove the old role if it's assigned
                if (!string.IsNullOrEmpty(oldRole) && currentRoles.Contains(oldRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, oldRole);
                }

                // Add the new role if not already assigned
                if (!string.IsNullOrEmpty(newRole) && !currentRoles.Contains(newRole))
                {
                    await _userManager.AddToRoleAsync(user, newRole);
                }

                TempData["SuccessMessage"] = "Role changed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "User not found.";
            }

            return RedirectToAction("CaseAdmin");
        }

    }
}
