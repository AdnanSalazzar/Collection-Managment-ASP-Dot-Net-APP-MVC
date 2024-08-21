using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CollectionManagement.Models.Domain;
using CollectionManagement.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;


namespace CollectionManagement.Controllers
{

    [Authorize(Roles = "Admin")]

    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;

        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();

            var model = users.Select(user => new AdminUserViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).Result
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
                IsUser = await _userManager.IsInRoleAsync(user, "User"),
                IsBlocked = user.IsBlocked
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model, string actionType)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            switch (actionType)
            {
                case "SaveRoles":
                    await UpdateRoles(user, model);
                    break;

                case "BlockUser":
                    user.IsBlocked = true;
                    await _userManager.UpdateAsync(user);
                    break;

                case "UnblockUser":
                    user.IsBlocked = false;
                    await _userManager.UpdateAsync(user);
                    break;

                case "DeleteUser":
                    await _userManager.DeleteAsync(user);
                    return RedirectToAction("Index"); // Redirect after deletion
            }

            return RedirectToAction("ManageRoles", new { userId = model.UserId });
        }

        private async Task UpdateRoles(User user, ManageUserRolesViewModel model)
        {
            // Update Admin role
            if (model.IsAdmin && !await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else if (!model.IsAdmin && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            // Update User role
            if (model.IsUser && !await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.AddToRoleAsync(user, "User");
            }
            else if (!model.IsUser && await _userManager.IsInRoleAsync(user, "User"))
            {
                await _userManager.RemoveFromRoleAsync(user, "User");
            }
        }
        //public async Task<IActionResult> ImpersonateUser(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Store the original admin ID in the session
        //    HttpContext.Session.SetString("AdminUserId", _userManager.GetUserId(User));

        //    // Sign out the admin
        //    await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

        //    // Sign in as the selected user
        //    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
        //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));  // Ensure user.Id is a string
        //    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

        //    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));

        //    return RedirectToAction("Index", "Collections");
        //}

        // Add this method to handle impersonation
        [HttpPost]
        public async Task<IActionResult> ImpersonateUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

           // Sign out the current admin
            await _signInManager.SignOutAsync();

            // Sign in as the selected user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Redirect to the Collections Index page as the impersonated user
            return RedirectToAction("Index", "Collections");
        }

        [AllowAnonymous]        
        public IActionResult StopImpersonation()
        {
            var originalAdminId = HttpContext.Session.GetString("AdminUserId");

            if (originalAdminId != null)
            {
                // Sign out the impersonated user
                HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                // Sign back in as the original admin
                var originalAdmin = _userManager.FindByIdAsync(originalAdminId).Result;
                var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, originalAdmin.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, originalAdmin.UserName));

                HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));

                // Clear the session variable
                HttpContext.Session.Remove("AdminUserId");

                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
