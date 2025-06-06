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
        private readonly TeacherService _teacherService;
        private Subject _currentSubject;
        private int _currentSemester = 1;

        public TeacherWindow(Teacher teacher, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _teacher = teacher;
            _dataService = dataService;
            _teacherService = new TeacherService(dataService);

            _currentSubject = _teacher.Subject ?? throw new InvalidOperationException("No subject assigned to teacher.");

            WelcomeText.Text = $"Welcome, {_teacher.FullName}!";
            SubjectNameText.Text = $"Subject: {_currentSubject.Name}";

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
            ExamDatePicker.SelectedDate = _currentSubject.ExamDate ?? DateTime.Today;
            RetakeDatePicker.SelectedDate = _currentSubject.RetakeDate ?? DateTime.Today;
            AssessmentTypeText.Text = _currentSubject.IsExam ? "Exam" : "Test";
        }

        private void ExamDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (!ExamDatePicker.SelectedDate.HasValue)
            {
                ExamDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void RetakeDatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (!RetakeDatePicker.SelectedDate.HasValue)
            {
                RetakeDatePicker.SelectedDate = DateTime.Today;
            }
        }

        private void LoadStudentGrades()
        {
            StudentsGrid.ItemsSource = _teacherService.GetStudentGrades(_currentSubject, _currentSemester);
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
                _teacherService.SaveGrades(studentGrades);
                LoadStudentGrades();
                MessageBox.Show("Grades saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
