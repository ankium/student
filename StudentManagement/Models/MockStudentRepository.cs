using System.Collections.Generic;
using System.Linq;
using StudentManagement.Models.EnumTypes;

namespace StudentManagement.Models
{
    public class MockStudentRepository : IStudentRepository
    {
        private readonly List<Student> studentList;
        public MockStudentRepository()
        {
            studentList = new List<Student>()
            {
                new Student(){Id=1,Name="张三",Major=MajorEnum.Programing,Email="sudons@msn.cn"},
                new Student(){Id=2,Name="李四",Major=MajorEnum.ComputerScience,Email="admin@ankium.com"},
                new Student(){Id=3,Name="王五",Major=MajorEnum.ElectronicCommerce,Email="student@school.com"},
                new Student(){Id=4,Name="陈六",Major=MajorEnum.Mathematics,Email="sudons@aliyun.com"}
            };
        }

        public Student GetStudentById(int? id)
        {
            return studentList.FirstOrDefault(stu => stu.Id == id);
        }

        public IEnumerable<Student> GetAllStudents()
        {
            return studentList;
        }

        public Student Insert(Student student)
        {
            student.Id = studentList.Max(stu=>stu.Id)+1;
            studentList.Add(student);
            return student;
        }

        public Student Update(Student updateStudent)
        {
            Student student = studentList.FirstOrDefault(stu => stu.Id == updateStudent.Id);
            if (student!=null)
            {
                student.Name = updateStudent.Name;
                student.Email = updateStudent.Email;
                student.Major = updateStudent.Major;
            }
            return student;
        }

        public Student Delete(int id)
        {
            Student student = studentList.FirstOrDefault(stu => stu.Id == id);
            if (student!=null)
            {
                studentList.Remove(student);
            }
            return student;
        }
    }
}