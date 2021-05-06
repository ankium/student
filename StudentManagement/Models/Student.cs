using StudentManagement.Models.EnumTypes;
using System.ComponentModel.DataAnnotations;
namespace StudentManagement.Models
{
    //学生模型
    public class Student
    {
        public int Id { get; set; }

        //[Required(ErrorMessage="姓名不能为空"),Display(Name="姓名")]
        //[MaxLength(50,ErrorMessage="姓名最大长度不能超过50个字符")]
        //[MinLength(2,ErrorMessage="姓名最小长度不能少于2个字符")]
        public string Name { get; set; }

        //[Display(Name="专业")]
        //[Required(ErrorMessage="请选择专业")]
        public MajorEnum? Major { get; set; }

        //[Display(Name="邮箱")]
        //[RegularExpression(@"^[a-zA-z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",ErrorMessage="邮箱格式不正确")]
        //[Required(ErrorMessage="邮箱不能为空")]
        public string Email { get; set; }
        public string PhotoPath { get; set; }
    }
}