using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StudentManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using StudentManagement.Extensions;

namespace StudentManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        //构造函数注册UserManager和SignInManager服务
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        #region 用户注册
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //将数据从RegisterViewModel复制到IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City,
                };
                //将用户数据存储在AspNetUser数据库表中
                var result = await userManager.CreateAsync(user, model.Password);
                //如果成功创建用户，则使用登录服务登录用户信息，并重定向到HomeController的index()操作方法中
                if (result.Succeeded)
                {
                    //如果用户已经登录且为Admin角色，那么就是Admin正在创建新用户，所以重定向Admin用户至ListUsers的视图列表
                    if (signInManager.IsSignedIn(User)&&User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }
                    //否则就是登录当前注册用户并重定向到HomeController的Index()操作方法中
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                //如果有任务错误，则将他们添加到ModelState对象中，将由验证摘要标记助手显示到视图中
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region 验证邮箱是否被使用
        [AcceptVerbs("Get","Post")]
        //[AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user==null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱:{email}已经被注册使用了。");
            }
        }
        #endregion

        #region 登录
        [HttpGet]
        //[AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model,string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        //检查是否为本地URL
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            //哪里来的登录后就回到地哪里去
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        //否则就回到首页
                        return RedirectToAction("index", "home");
                    }
                }
                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }
            return View(model);
        }
        #endregion

        #region 注销
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        #endregion

        #region 访问被拒绝
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion
    }
}
