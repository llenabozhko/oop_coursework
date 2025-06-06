using System;
using System.Collections.Generic;
using System.Linq;
using oop_coursework.Models;

namespace oop_coursework.Services
{
    public class TeacherService
    {
        private readonly DataService _dataService;

        public TeacherService(DataService dataService)
        {
            _dataService = dataService;
        }

        public List<StudentGradeViewModel> GetStudentGrades(Subject subject, int semester)
        {
            var students = _dataService.GetStudents();
            var grades = _dataService.GetGrades()
                .Where(g => g.SubjectId == subject.Id && g.Semester == semester)
                .ToList();

            var studentGrades = new List<StudentGradeViewModel>();

            foreach (var student in students)
            {
                var grade = grades.FirstOrDefault(g => g.StudentId == student.Id) ??
                    new Grade
                    {
                        StudentId = student.Id,
                        SubjectId = subject.Id,
                        Score = 0,
                        Subject = subject,
                        Semester = semester
                    };

                studentGrades.Add(new StudentGradeViewModel
                {
                    Student = student,
                    Grade = grade
                });
            }

            return studentGrades;
        }

        public void SaveGrades(List<StudentGradeViewModel> studentGrades)
        {
            foreach (var vm in studentGrades)
            {
                if (vm.Grade.Id == 0)
                {
                    _dataService.AddGrade(vm.Grade);
                }
            }

            _dataService.SaveChanges();
        }
    }

    public class StudentGradeViewModel
    {
        public required Student Student { get; set; }
        public required Grade Grade { get; set; }
        public string MainExamScore => Grade.Score > 0 ? Grade.Score.ToString("F1") : "-";
        public string RetakeScore => Grade.RetakeScore.HasValue ? Grade.RetakeScore.Value.ToString("F1") : "-";
        public string FinalScore => Grade.FinalScore.ToString("F1");
    }
}
