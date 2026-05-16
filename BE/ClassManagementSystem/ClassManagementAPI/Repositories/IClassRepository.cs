using ClassManagementAPI.Models;

namespace ClassManagementAPI.Repositories
{
    public interface IClassRepository
    {
        IEnumerable<Class> GetClassesByTeacherId(int teacherId);
        Class GetClassById(int id);
        void AddClass(Class classObj);
        void UpdateClass(Class classObj);
        void DeleteClass(Class classObj);
        
        // Student Management
        void AddStudentToClass(ClassStudent cs);
        void RemoveStudentFromClass(ClassStudent cs);
        ClassStudent GetClassStudent(int classId, int studentId);
        IEnumerable<User> GetStudentsInClass(int classId);
        IEnumerable<Class> GetClassesForStudent(int studentId);
        
        void SaveChanges();
    }
}
