using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Models;
using StudentManagement.Models.EnumTypes;

namespace StudentManagement.Infrastructure
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 1,
                    Name = "云文",
                    Major = MajorEnum.Programing,
                    Email = "sudons@msn.cn",
                }
            );
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 2,
                    Name = "王子",
                    Major = MajorEnum.ComputerScience,
                    Email = "sudons@aliyun.com",
                }
            );
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    Id = 3,
                    Name = "小胡",
                    Major = MajorEnum.Mathematics,
                    Email = "admin@msn.cn"
                }
            );
        }
    }
}
