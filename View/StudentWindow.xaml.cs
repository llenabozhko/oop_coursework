using System.Collections.Generic;
using System.Linq;
using System.Windows;
using oop_coursework.Models;
using oop_coursework.Services;

namespace oop_coursework.Views
{
    public partial class StudentWindow : BaseWindow
    {
        private readonly Student _student;
        private readonly DataService _dataService;

        public class GradeViewModel
        {
            public Subject Subject { get; set; }
            public double? Score { get; set; }
        }

        public StudentWindow(Student student, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _student = student;
            _dataService = dataService;

            WelcomeText.Text = $"Welcome, {_student.FullName}!";
            SpecialtyText.Text = $"Specialty: {_student.Specialty}";

            LoadGrades();
        }

        private void LoadGrades()
        {
            var subjects = _dataService.GetSubjects();
            var grades = _dataService.GetGrades()
                .Where(g => g.StudentId == _student.Id)
                .ToList();

            var gradeViewModels = new List<GradeViewModel>();

            foreach (var subject in subjects)
            {
                var grade = grades.FirstOrDefault(g => g.SubjectId == subject.Id);
                gradeViewModels.Add(new GradeViewModel
                {
                    Subject = subject,
                    Score = grade?.Score
                });
            }

            GradesGrid.ItemsSource = gradeViewModels;
        }
    }
}
