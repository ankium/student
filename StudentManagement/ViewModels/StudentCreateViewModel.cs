using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StudentManagement.Models.EnumTypes;

namespace StudentManagement.ViewModels
{
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "姓名不能为空"), Display(Name = "姓名")]
        [MaxLength(50, ErrorMessage = "姓名最大长度不能超过50个字符")]
        [MinLength(2, ErrorMessage = "姓名最小长度不能少于2个字符")]
        public string Name { get; set; }

        [Display(Name = "专业")]
        [Required(ErrorMessage = "请选择专业")]
        public MajorEnum? Major { get; set; }

        [Display(Name = "邮箱")]
        [RegularExpression(@"^[a-zA-z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "邮箱格式不正确")]
        [Required(ErrorMessage = "邮箱不能为空")]
        public string Email { get; set; }
        
        [Display(Name = "头像")]
        public IFormFile Photo { get; set; }

        [Display(Name="头像")]
        public List<IFormFile> Photos { get; set; }
    }
}
