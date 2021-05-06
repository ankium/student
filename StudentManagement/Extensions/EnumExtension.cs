using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StudentManagement.Extensions
{
    /// <summary>
    /// 枚举的扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的显示名字
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string GetDisplayName(this System.Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memberInfo = type.GetMember(en.ToString());
            if (memberInfo!=null&&memberInfo.Length>0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), true);
                if (attrs!=null&&attrs.Length>0)
                {
                    return ((DisplayAttribute)attrs[0]).Name;
                }
            }
            return en.ToString();
        }
    }
}
