using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Authorize(Roles ="admin")]
    public class AdminController: Controller
    {   
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _user;
        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<IdentityUser> user)
        {
            _user = user;
            _roleManager = roleManager;
            
        }

        [HttpGet]
        public IActionResult CreateRole() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(Role role) 
        {
            if(ModelState.IsValid){
                IdentityRole identityRole= new IdentityRole {
                    Name=role.RoleName
                };

                IdentityResult result= await _roleManager.CreateAsync(identityRole);
            
            if(result.Succeeded)
            {
                return RedirectToAction("ListRoles","Admin");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("",error.Description);
            }
            }

            
            return View(role);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles= _roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role= await _roleManager.FindByIdAsync(id);

            if(role==null)
            {
                return NotFound();
            }

            var model= new EditRole
            {
                Id=role.Id,
                RoleName=role.Name!
                

            };
            foreach (var user in await _user.Users.ToListAsync())
                {                    
                   if (await _user.IsInRoleAsync(user, role.Name!))
                   {
                       model.Users.Add(user.UserName!);
                   }         
                }


            return View(model);
        
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRole model)
        {
            var role= await _roleManager.FindByIdAsync(model.Id!);

            if(role==null) return NotFound();

            else 
            {
                role.Name=model.RoleName;
                var result=await _roleManager.UpdateAsync(role);
                if(result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("",error.Description);
                }
            
            return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId=roleId;

            var role=await _roleManager.FindByIdAsync(roleId);
            if(role==null) return NotFound();

            var model= new List<UserRole>();

            foreach (var user in _user.Users)
            {
                var userRole= new UserRole
                {
                    UserId=user.Id,
                    UserName=user.UserName!
                };
                if(await _user.IsInRoleAsync(user,role.Name!)) userRole.IsSelected=true;
                else userRole.IsSelected=false;

                model.Add(userRole);
            }


            return View(model);

        }
        [HttpPost]
public async Task<IActionResult> EditUsersInRole(List<UserRole> model, string roleId)
{
    var role = await _roleManager.FindByIdAsync(roleId);

    if (role == null) return NotFound();

    for (int i = 0; i < model.Count; i++)
    {
        var user = await _user.FindByIdAsync(model[i].UserId!);

        IdentityResult? result = null;

        if (model[i].IsSelected && !(await _user.IsInRoleAsync(user!, role.Name!)))
        {
            result = await _user.AddToRoleAsync(user!, role.Name!);
        }
        else if (!model[i].IsSelected && await _user.IsInRoleAsync(user!, role.Name!))
        {
            if(user!.UserName!="test@test.com")
            result = await _user.RemoveFromRoleAsync(user, role.Name!);
            else continue;
        }
        else
        {
            continue;
        }

        if (result.Succeeded)
        {
            if (i < (model.Count - 1))
                continue;
            else
                return RedirectToAction("EditRole", new { Id = roleId });
        }
    }

    return RedirectToAction("EditRole", new { Id = roleId });
}
        
        
    }
}