using System.Collections.Generic;
namespace StudentManagement.Models
{
    public interface IStudentRepository
    {
        //通过ID获取学生信息
         Student GetStudentById(int? id);
        //获取所有学生信息
         IEnumerable<Student> GetAllStudents();
        //添加学生信息
         Student Insert(Student student);
        //修改学生信息
        Student Update(Student student);
        //删除学生信息
        Student Delete(int id);
    }
}