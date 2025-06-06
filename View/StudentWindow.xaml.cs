using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using oop_coursework.Models;
using oop_coursework.Services;

namespace oop_coursework.Views
{
    public partial class StudentWindow : BaseWindow
    {
        private readonly Student _student;
        private readonly DataService _dataService;
        private readonly StudentService _studentService;
        private List<string> _alerts;

        public StudentWindow(Student student, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _student = student;
            _dataService = dataService;
            _alerts = new List<string>();
            _studentService = new StudentService(dataService);

            WelcomeText.Text = $"Вітаємо, {_student.FullName}!";
            SpecialtyText.Text = $"Спеціальність: {_student.Specialty}";

            SemesterTabs.SelectionChanged += SemesterTabs_SelectionChanged;
            LoadGrades();
        }

        private void SemesterTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SemesterTabs.SelectedItem is TabItem selectedTab)
            {
                int semester = selectedTab.Header.ToString() == "Семестр 1" ? 1 : 2;
                LoadGrades(semester);
            }
        }

        private void LoadGrades(int semester = 1)
        {
            var subjects = _dataService.GetSubjects();
            var grades = _dataService.GetGrades()
                .Where(g => g.StudentId == _student.Id && g.Semester == semester)
                .ToList();

            var gradeViewModels = new List<Grade>();
            _alerts.Clear();
            _alerts = _studentService.GenerateAlerts(_student, semester);

            foreach (var subject in subjects)
            {
                var grade = grades.FirstOrDefault(g => g.SubjectId == subject.Id) ??
                    new Grade
                    {
                        Subject = subject,
                        Score = 0,
                        Semester = semester
                    };

                gradeViewModels.Add(grade);
            }

            GradesGrid.ItemsSource = gradeViewModels;

            if (_alerts.Any())
            {
                AlertsPanel.Visibility = Visibility.Visible;
                AlertsList.ItemsSource = _alerts;
            }
            else
            {
                AlertsPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
