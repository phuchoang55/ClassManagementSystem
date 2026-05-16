using ClassManagementAPI.DTOs;
using ClassManagementAPI.Models;
using ClassManagementAPI.Repositories;

namespace ClassManagementAPI.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IUserRepository _userRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public ClassService(IClassRepository classRepository, IUserRepository userRepository,
            IScheduleRepository scheduleRepository, IAttendanceRepository attendanceRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
            _attendanceRepository = attendanceRepository;
        }

        public IEnumerable<object> GetTeacherClasses(int teacherId)
        {
            var classes = _classRepository.GetClassesByTeacherId(teacherId);
            return classes.Select(c => new { c.Id, c.Name, c.Description, c.CreatedAt, c.StartDate, c.EndDate });
        }

        public object CreateClass(int teacherId, ClassCreateDto request)
        {
            var newClass = new Class
            {
                Name = request.Name,
                Description = request.Description,
                TeacherId = teacherId,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            _classRepository.AddClass(newClass);
            _classRepository.SaveChanges();

            return new { newClass.Id, newClass.Name, newClass.Description, newClass.CreatedAt, newClass.StartDate, newClass.EndDate };
        }

        public object UpdateClass(int teacherId, int classId, ClassUpdateDto request)
        {
            var existingClass = _classRepository.GetClassById(classId);
            if (existingClass == null)
                throw new Exception("Không tìm thấy lớp học.");
            
            if (existingClass.TeacherId != teacherId)
                throw new Exception("Bạn không có quyền chỉnh sửa lớp học này.");

            existingClass.Name = request.Name;
            existingClass.Description = request.Description;
            existingClass.StartDate = request.StartDate;
            existingClass.EndDate = request.EndDate;

            _classRepository.UpdateClass(existingClass);
            _classRepository.SaveChanges();

            return new { existingClass.Id, existingClass.Name, existingClass.Description, existingClass.CreatedAt, existingClass.StartDate, existingClass.EndDate };
        }

        public void DeleteClass(int teacherId, int classId)
        {
            var existingClass = _classRepository.GetClassById(classId);
            if (existingClass == null)
                throw new Exception("Không tìm thấy lớp học.");
            
            if (existingClass.TeacherId != teacherId)
                throw new Exception("Bạn không có quyền xóa lớp học này.");

            _classRepository.DeleteClass(existingClass);
            _classRepository.SaveChanges();
        }

        // Student Management
        public object AddStudent(int teacherId, int classId, string studentEmail)
        {
            var cls = _classRepository.GetClassById(classId);
            if (cls == null || cls.TeacherId != teacherId)
                throw new Exception("Lớp học không tồn tại hoặc bạn không có quyền.");

            var student = _userRepository.GetUserByEmail(studentEmail);
            if (student == null || student.Role == "Teacher" || student.Role == "Admin")
                throw new Exception("Không tìm thấy học sinh với email này.");

            var existingEnrollment = _classRepository.GetClassStudent(classId, student.Id);
            if (existingEnrollment != null)
                throw new Exception("Học sinh này đã ở trong lớp.");

            var cs = new ClassStudent
            {
                ClassId = classId,
                StudentId = student.Id
            };

            _classRepository.AddStudentToClass(cs);
            _classRepository.SaveChanges();

            return new { student.Id, student.FullName, student.Email, cs.EnrolledAt };
        }

        public void RemoveStudent(int teacherId, int classId, int studentId)
        {
            var cls = _classRepository.GetClassById(classId);
            if (cls == null || cls.TeacherId != teacherId)
                throw new Exception("Lớp học không tồn tại hoặc bạn không có quyền.");

            var cs = _classRepository.GetClassStudent(classId, studentId);
            if (cs == null)
                throw new Exception("Học sinh này không ở trong lớp.");

            _classRepository.RemoveStudentFromClass(cs);
            _classRepository.SaveChanges();
        }

        public IEnumerable<object> GetStudents(int teacherId, int classId)
        {
            var cls = _classRepository.GetClassById(classId);
            if (cls == null || cls.TeacherId != teacherId)
                throw new Exception("Lớp học không tồn tại hoặc bạn không có quyền.");

            var students = _classRepository.GetStudentsInClass(classId);
            return students.Select(s => new { s.Id, s.FullName, s.Email });
        }

        public IEnumerable<object> GetStudentEnrolledClasses(int studentId)
        {
            var classes = _classRepository.GetClassesForStudent(studentId);
            return classes.Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.CreatedAt,
                c.StartDate,
                c.EndDate,
                TeacherName = _userRepository.GetUserById(c.TeacherId)?.FullName
            });
        }

        public object GetStudentClassDetail(int studentId, int classId)
        {
            // Kiểm tra student có trong lớp không
            var enrollment = _classRepository.GetClassStudent(classId, studentId);
            if (enrollment == null)
                throw new Exception("Bạn không thuộc lớp học này.");

            var cls = _classRepository.GetClassById(classId);
            if (cls == null)
                throw new Exception("Không tìm thấy lớp học.");

            var teacher = _userRepository.GetUserById(cls.TeacherId);

            var rawSchedules = _scheduleRepository.GetSchedulesByClassId(classId).ToList();

            // Lịch học
            var schedules = rawSchedules
                .Select(s => new
                {
                    s.Id,
                    s.DayOfWeek,
                    StartTime = s.StartTime.ToString(@"hh\:mm"),
                    EndTime   = s.EndTime.ToString(@"hh\:mm"),
                    s.Room
                }).ToList();

            // Điểm danh thực tế của riêng student này
            var actualAttendances = _attendanceRepository.GetByClass(classId)
                .Where(a => a.StudentId == studentId)
                .ToList();

            var attendancesResult = new List<object>();

            if (cls.StartDate.HasValue && cls.EndDate.HasValue && rawSchedules.Any())
            {
                var currentDate = cls.StartDate.Value.Date;
                var endDate = cls.EndDate.Value.Date;
                var sessionList = new List<(int Id, string Date, string Status, int? ScheduleId)>();

                while (currentDate <= endDate)
                {
                    var currentDayOfWeek = currentDate.DayOfWeek;
                    var matchingSchedules = rawSchedules.Where(s => s.DayOfWeek == currentDayOfWeek);

                    foreach (var sch in matchingSchedules)
                    {
                        var existingRecord = actualAttendances.FirstOrDefault(a => a.Date.Date == currentDate && (a.ScheduleId == sch.Id || a.ScheduleId == null));
                        sessionList.Add((
                            existingRecord?.Id ?? 0,
                            currentDate.ToString("yyyy-MM-dd"),
                            existingRecord?.Status ?? "Not yet",
                            sch.Id
                        ));
                    }
                    currentDate = currentDate.AddDays(1);
                }

                attendancesResult = sessionList.OrderByDescending(s => s.Date).Select(s => new
                {
                    s.Id,
                    s.Date,
                    s.Status,
                    s.ScheduleId
                }).ToList<object>();
            }
            else
            {
                attendancesResult = actualAttendances.OrderByDescending(a => a.Date).Select(a => new
                {
                    a.Id,
                    Date = a.Date.ToString("yyyy-MM-dd"),
                    a.Status,
                    a.ScheduleId
                }).ToList<object>();
            }

            return new
            {
                cls.Id,
                cls.Name,
                cls.Description,
                cls.StartDate,
                cls.EndDate,
                cls.CreatedAt,
                TeacherName = teacher?.FullName ?? "Không rõ",
                Schedules   = schedules,
                Attendances = attendancesResult
            };
        }
    }
}
