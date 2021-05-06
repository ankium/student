using System.ComponentModel.DataAnnotations;
namespace StudentManagement.Models.EnumTypes
{
    public enum MajorEnum
    {
        [Display(Name="未分配")]
        None,
        [Display(Name="软件开发")]
        Programing,
        [Display(Name="计算机科学")]
        ComputerScience,
        [Display(Name="电子商务")]
        ElectronicCommerce,
        [Display(Name="高等数学")]
        Mathematics
    }
}