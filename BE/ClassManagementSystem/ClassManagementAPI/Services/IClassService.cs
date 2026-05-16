using ClassManagementAPI.DTOs;

namespace ClassManagementAPI.Services
{
    public interface IClassService
    {
        IEnumerable<object> GetTeacherClasses(int teacherId);
        object CreateClass(int teacherId, ClassCreateDto request);
        object UpdateClass(int teacherId, int classId, ClassUpdateDto request);
        void DeleteClass(int teacherId, int classId);

        // Student Management
        object AddStudent(int teacherId, int classId, string studentEmail);
        void RemoveStudent(int teacherId, int classId, int studentId);
        IEnumerable<object> GetStudents(int teacherId, int classId);
        
        IEnumerable<object> GetStudentEnrolledClasses(int studentId);
        object GetStudentClassDetail(int studentId, int classId);
    }
}
