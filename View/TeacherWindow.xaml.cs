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
        private Subject _currentSubject;
        private int _currentSemester = 1;

        public class StudentGradeViewModel
        {
            public required Student Student { get; set; }
            public required Grade Grade { get; set; }
            public string MainExamScore => Grade.Score > 0 ? Grade.Score.ToString("F1") : "-";
            public string RetakeScore => Grade.RetakeScore.HasValue ? Grade.RetakeScore.Value.ToString("F1") : "-";
            public string FinalScore => Grade.FinalScore.ToString("F1");
        }

        public TeacherWindow(Teacher teacher, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _teacher = teacher;
            _dataService = dataService;

            // Hide subject selection as teacher has only one subject
            SubjectSelectionPanel.Visibility = Visibility.Collapsed;

            _currentSubject = _teacher.Subject;
            if (_currentSubject == null)
            {
                MessageBox.Show("No subject assigned to teacher.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            WelcomeText.Text = $"Welcome, {_teacher.FullName}!";
            SubjectNameText.Text = $"Subject: {_currentSubject.Name}";

            // Setup semester selection
            SemesterComboBox.SelectedIndex = 0;
            SemesterComboBox.SelectionChanged += SemesterComboBox_SelectionChanged;

            LoadSubjectSettings();
            LoadStudentGrades();
        }

        private void SemesterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SemesterComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                _currentSemester = int.Parse(selectedItem.Content.ToString() ?? "1");
                LoadStudentGrades();
            }
        }

        private void LoadSubjectSettings()
        {
            ExamDatePicker.SelectedDate = _currentSubject.ExamDate;
            RetakeDatePicker.SelectedDate = _currentSubject.RetakeDate;
            AssessmentTypeText.Text = _currentSubject.IsExam ? "Exam" : "Test";
        }

        private void LoadStudentGrades()
        {
            var students = _dataService.GetStudents();
            var grades = _dataService.GetGrades()
                .Where(g => g.SubjectId == _currentSubject.Id && g.Semester == _currentSemester)
                .ToList();

            var studentGrades = new List<StudentGradeViewModel>();

            foreach (var student in students)
            {
                var grade = grades.FirstOrDefault(g => g.StudentId == student.Id) ??
                    new Grade
                    {
                        StudentId = student.Id,
                        SubjectId = _currentSubject.Id,
                        Score = 0,
                        Subject = _currentSubject,
                        Semester = _currentSemester
                    };

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

            _currentSubject.ExamDate = ExamDatePicker.SelectedDate.Value;

            if (RetakeDatePicker.SelectedDate.HasValue)
            {
                if (RetakeDatePicker.SelectedDate.Value <= _currentSubject.ExamDate)
                {
                    MessageBox.Show("Retake date must be after the main exam date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                _currentSubject.RetakeDate = RetakeDatePicker.SelectedDate.Value;
            }

            _dataService.SaveChanges();
            LoadSubjectSettings();

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

                    // Enable retake tab if grade is below 60
                    if (vm.Grade.Score < 60)
                    {
                        RetakeTab.IsEnabled = true;
                    }

                    if (vm.Grade.Id == 0)
                    {
                        _dataService.AddGrade(vm.Grade);
                    }
                }

                _dataService.SaveChanges();
                LoadStudentGrades();
                MessageBox.Show("Grades saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
