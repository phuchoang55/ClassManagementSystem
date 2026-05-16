using ClassManagementAPI.Data;
using ClassManagementAPI.Models;

namespace ClassManagementAPI.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly AppDbContext _context;

        public ClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Class> GetClassesByTeacherId(int teacherId)
        {
            return _context.Classes.Where(c => c.TeacherId == teacherId).ToList();
        }

        public Class GetClassById(int id)
        {
            return _context.Classes.FirstOrDefault(c => c.Id == id);
        }

        public void AddClass(Class classObj)
        {
            _context.Classes.Add(classObj);
        }

        public void UpdateClass(Class classObj)
        {
            _context.Classes.Update(classObj);
        }

        public void DeleteClass(Class classObj)
        {
            _context.Classes.Remove(classObj);
        }

        // Student Management
        public void AddStudentToClass(ClassStudent cs)
        {
            _context.ClassStudents.Add(cs);
        }

        public void RemoveStudentFromClass(ClassStudent cs)
        {
            _context.ClassStudents.Remove(cs);
        }

        public ClassStudent GetClassStudent(int classId, int studentId)
        {
            return _context.ClassStudents.FirstOrDefault(cs => cs.ClassId == classId && cs.StudentId == studentId);
        }

        public IEnumerable<User> GetStudentsInClass(int classId)
        {
            var studentIds = _context.ClassStudents
                .Where(cs => cs.ClassId == classId)
                .Select(cs => cs.StudentId)
                .ToList();

            return _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToList();
        }

        public IEnumerable<Class> GetClassesForStudent(int studentId)
        {
            var classIds = _context.ClassStudents
                .Where(cs => cs.StudentId == studentId)
                .Select(cs => cs.ClassId)
                .ToList();

            return _context.Classes
                .Where(c => classIds.Contains(c.Id))
                .ToList();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
