using EShop.Domain.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EShop.Web.Controllers
{
    public class SwitchRolesController : Controller
    {
        private readonly UserManager<EShopApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;


        public SwitchRolesController(UserManager<EShopApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userc = await userManager.FindByIdAsync(userId);

            var standardRole = await roleManager.FindByNameAsync("STANDARD");

            var currentRoles = await userManager.GetRolesAsync(userc);


            if (standardRole == null)
            {
                // Role not found, handle the situation
                return null;
            }

            var users = userManager.Users.ToList();
            var usersWithStandardRole = new List<EShopApplicationUser>();

            foreach (var user in users)
            {
                if (await userManager.IsInRoleAsync(user, standardRole.Name))
                {
                    usersWithStandardRole.Add(user);
                }
            }

            ViewBag.users = usersWithStandardRole;

            return View();
        }

        public async Task<IActionResult> ChangeRoleForm(Guid? id)
        {
            ViewBag.userId = id;

            return View();
        } 

        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string role)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Handle invalid user ID
                return RedirectToAction("Index", "Home");
            }

            // Remove all existing roles
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign the new role
            await userManager.AddToRoleAsync(user, role);

            return RedirectToAction("Index", "Home");
        }

    }

}
