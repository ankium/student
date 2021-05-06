using Microsoft.AspNetCore.Mvc;
using StudentManagement.CustomMiddlewares.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.ViewModels
{
    public class RegisterViewModel
    {
        [Required,EmailAddress,Display(Name="邮箱"),Remote(action:"IsEmailInUse",controller:"Account")]
        //应用自定义验证属性
        [ValidEmailDomain(allowedDomain:"msn.cn",ErrorMessage ="邮箱地址的后缀必须是msn.cn")]
        public string Email { get; set; }

        [Required,DataType(DataType.Password),Display(Name ="密码")]
        public string Password { get; set; }

        [DataType(DataType.Password),Display(Name ="确认密码"),Compare("Password",ErrorMessage ="密码与确认密码不壹致，请重新输入")]
        public string ConfirmPassword { get; set; }

        [Display(Name ="城市")]
        public string City { get; set; }
    }
}
