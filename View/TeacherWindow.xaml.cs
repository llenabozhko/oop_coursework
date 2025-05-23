using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using oop_coursework.Models;
using oop_coursework.Services;

namespace oop_coursework.Views
{
    public partial class TeacherWindow : BaseWindow
    {
        private readonly Teacher _teacher;
        private readonly DataService _dataService;

        public class StudentGradeViewModel
        {
            public Student Student { get; set; }
            public Grade Grade { get; set; }
        }

        public TeacherWindow(Teacher teacher, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _teacher = teacher;
            _dataService = dataService;

            // Initialize subject if it doesn't exist
            if (_teacher.Subject == null)
            {
                var existingSubject = _dataService.GetSubjects()
                    .FirstOrDefault(s => s.Name == "Mathematics"); // Default to Math if no subject assigned

                if (existingSubject != null)
                {
                    _teacher.Subject = existingSubject;
                }
                else
                {
                    _teacher.Subject = new MathSubject
                    {
                        ExamDate = DateTime.Now.AddMonths(1),
                        Credits = 5,
                        IsExam = true
                    };
                    _dataService.AddSubject(_teacher.Subject);
                }
                _dataService.SaveChanges();
            }

            WelcomeText.Text = $"Welcome, {_teacher.FullName}!";
            SubjectNameText.Text = _teacher.Subject.Name;

            LoadSubjectSettings();
            LoadStudentGrades();
        }

        private void LoadSubjectSettings()
        {
            ExamDatePicker.SelectedDate = _teacher.Subject.ExamDate;
            AssessmentTypeComboBox.SelectedIndex = _teacher.Subject.IsExam ? 0 : 1;
        }

        private void LoadStudentGrades()
        {
            var students = _dataService.GetStudents();
            var grades = _dataService.GetGrades()
                .Where(g => g.SubjectId == _teacher.Subject.Id)
                .ToList();

            var studentGrades = new List<StudentGradeViewModel>();

            foreach (var student in students)
            {
                var grade = grades.FirstOrDefault(g => g.StudentId == student.Id) ??
                    new Grade { StudentId = student.Id, SubjectId = _teacher.Subject.Id, Score = 0 };
                grade.Subject = _teacher.Subject;

                studentGrades.Add(new StudentGradeViewModel
                {
                    Student = student,
                    Grade = grade
                });
            }

            StudentsGrid.ItemsSource = studentGrades;
        }

        private void UpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            if (!ExamDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an exam date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _teacher.Subject.ExamDate = ExamDatePicker.SelectedDate.Value;
            _teacher.Subject.IsExam = AssessmentTypeComboBox.SelectedIndex == 0;
            _dataService.SaveChanges();

            MessageBox.Show("Settings updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveGrades_Click(object sender, RoutedEventArgs e)
        {
            if (StudentsGrid.ItemsSource is List<StudentGradeViewModel> studentGrades)
            {
                foreach (var vm in studentGrades)
                {
                    if (vm.Grade.Score < 0 || vm.Grade.Score > 100)
                    {
                        MessageBox.Show($"Invalid grade for student {vm.Student.FullName}. Grade must be between 0 and 100.",
                            "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (vm.Grade.Id == 0)
                    {
                        _dataService.AddGrade(vm.Grade);
                    }
                }

                _dataService.SaveChanges();
                MessageBox.Show("Grades saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
