using System.Collections.Generic;
namespace StudentManagement.Models
{
    public interface IStudentRepository
    {
        //ͨ��ID��ȡѧ����Ϣ
         Student GetStudentById(int? id);
        //��ȡ����ѧ����Ϣ
         IEnumerable<Student> GetAllStudents();
        //���ѧ����Ϣ
         Student Insert(Student student);
        //�޸�ѧ����Ϣ
        Student Update(Student student);
        //ɾ��ѧ����Ϣ
        Student Delete(int id);
    }
}