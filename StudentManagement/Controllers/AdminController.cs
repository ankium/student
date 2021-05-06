using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.ViewModels;
using StudentManagement.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using StudentManagement.Models;

namespace StudentManagement.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminController> logger;
        public AdminController(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager,ILogger<AdminController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;

        }

        #region 用户列表
        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users.ToList();
            return View(users);
        }
        #endregion

        #region 编辑用户
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的用户";
                return View("NotFound");
            }
            //GetClaimsAsync()返回用户声明列表
            var userClaims = await userManager.GetClaimsAsync(user);
            //GetRolesAsync()返回用户角色列表
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = (List<string>)userRoles
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{model.Id}的用户";
                return View("NotFound");
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.City = model.City;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }
        #endregion

        #region 删除用户
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的用户";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("ListUsers");
            }
        }
        #endregion

        #region 编辑用户中的角色
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{userId}的用户";
                return View("NotFound");
            }
            var model = new List<RolesInUserViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var rolesInUserViewModel = new RolesInUserViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                //判断当前用户是否已经拥有该角色信息
                if (await userManager.IsInRoleAsync(user,role.Name))
                {
                    rolesInUserViewModel.IsSelected = true;
                }
                else
                {
                    rolesInUserViewModel.IsSelected = false;
                }
                //添加已有角色到视图模型
                model.Add(rolesInUserViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<RolesInUserViewModel> model,string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到用户ID为{userId}的用户";
                return View("NotFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            //移除当前用户中的所有角色信息
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "无法删除现有用户中的角色");
                return View(model);
            }
            //查询模型列表中被选中的RoleName并添加到用户中
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "无法向用户中添加选定的角色");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = userId });
        }
        #endregion

        #region 角色列表
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        #endregion

        #region 创建角色
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateRole( CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //我们只需要指定一个不重复的角色名来创建新角色
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                //将角色保存在AspNetRoles表中
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("listroles", "admin");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region 编辑角色

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"角色ID={id}的信息不存在，请重试。";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            var users = userManager.Users.ToList();
            //查询所有用户
            foreach (var user in users)
            {
                //如果用户拥有此角色，请将用户添加到EditRoleViewModel中的Users属性中，然后将该对象传递给视图显示到客户端
                if (await userManager.IsInRoleAsync(user,role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"角色ID={model.Id}的信息不存在，请重试。";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                //使用UpdateAsync()更新角色
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("listroles");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }

        #endregion

        #region 删除角色
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的角色信息";
                return View("NotFound");
            }
            else
            {
                //将代码包装在try catch中
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("ListRoles");
                }
                //如果触发的异常是DbUpdateException，则知道我们无法删除角色，因为该角色中已经存在用户信息
                catch (DbUpdateException ex)
                {

                    //将异常记录到日志文件中，使用NLog配置日志信息
                    logger.LogError($"发生异常：{ex}");
                    //通过ViewBag.ErrorTitle和ViewBag.ErrorMessage将错误标题和错误信息传递给错误视图，错误视图会将这些数据展示给用户
                    ViewBag.ErrorTitle = $"角色：{role.Name}正在被使用中！";
                    ViewBag.ErrorMessage = $"无法删除{role.Name}角色，因为此角色中已经存在用户。如果读者想删除此角色，需要先从该角色中删除用户，然后尝试删除该角色本身。";
                    return View("Error");
                }
            }
        }
        #endregion

        #region 编辑角色中的用户

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            //通过roleId查询角色实体信息
            var role = await roleManager.FindByIdAsync(roleId);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"角色ID={roleId}的信息不存在，请重试。";
                return View("NotFound");
            }
            var model = new List<UserRoleViewModel>();
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                //判断当前用户是否已经存在角色中
                var isInRole = await userManager.IsInRoleAsync(user, role.Name);
                if (isInRole)
                {
                    //存在则设置为选中状态，值为true
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    //不存在则设置为非选中状态，值为false
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model,string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role==null)
            {
                ViewBag.ErrorMessage = $"角色ID={roleId}的信息不存在，请重试。";
                return View("NotFound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                //检查当前的UserId是否被选中，如果被选中了，则添加到角色列表中
                if (model[i].IsSelected&&!(await userManager.IsInRoleAsync(user,role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                //如果没有选中，则从UserRoles表中移除
                else if(!model[i].IsSelected&&await userManager.IsInRoleAsync(user,role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                //对于其它情况不做处理，继续新的循环
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    //判断当前用户是否为最后壹个用户，如果是，则跳转到EditRole视图
                    if (i<model.Count-1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("editrole", new { Id = roleId });
                    }
                }
            }
            return RedirectToAction("editrole", new { Id = roleId });
        }
        #endregion

        #region 管理用户声明
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{userId}的用户";
                return View("NotFound");
            }
            //UserManager服务中的GetClaimsAsync()方法获取用户当前的所有声明
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };
            //循环遍历应用程序中的每个声明
            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                //如果用户选中了声明属性，则设置IsSelected属性为true
                if (existingUserClaims.Any(c=>c.Type==claim.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.Cliams.Add(userClaim);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user==null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{model.UserId}的用户";
                return View("NotFound");
            }
            //获取所有用户现有的声明并删除它们
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "无法删除当前用户的声明");
                return View(model);
            }
            //添加页面上所选中的声明信息
            result = await userManager.AddClaimsAsync(user, model.Cliams.Where(c=>c.IsSelected).Select(c=>new Claim(c.ClaimType,c.ClaimType)));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "无法向用户添加选定的声明");
                return View(model);
            }
            return RedirectToAction("EditUser", new { Id = model.UserId });
        }
        #endregion
    }
}
