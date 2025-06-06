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
        private List<string> _alerts;

        public StudentWindow(Student student, DataService dataService)
        {
            InitializeComponent();
            InitializeMenu();

            _student = student;
            _dataService = dataService;
            _alerts = new List<string>();

            WelcomeText.Text = $"Welcome, {_student.FullName}!";
            SpecialtyText.Text = $"Specialty: {_student.Specialty}";

            SemesterTabs.SelectionChanged += SemesterTabs_SelectionChanged;
            LoadGrades();
        }

        private void SemesterTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SemesterTabs.SelectedItem is TabItem selectedTab)
            {
                int semester = selectedTab.Header.ToString() == "Semester 1" ? 1 : 2;
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

            foreach (var subject in subjects)
            {
                var grade = grades.FirstOrDefault(g => g.SubjectId == subject.Id) ??
                    new Grade
                    {
                        Subject = subject,
                        Score = 0,
                        Semester = semester
                    };

                // Проверяем уведомления только если это экзамен
                if (subject.IsExam)
                {
                    // Если экзамен скоро и оценки еще нет
                    if (subject.IsExamSoon && grade.Score == 0)
                    {
                        _alerts.Add($"Upcoming exam in {subject.Name} on {subject.ExamDate:d}!");
                    }
                    // Если нужна пересдача (оценка < 35)
                    else if (subject.NeedsRetake(grade.Score))
                    {
                        if (subject.IsRetakeAvailable(grade.Score))
                        {
                            if (subject.IsRetakeSoon(grade.Score))
                            {
                                _alerts.Add($"URGENT: Retake exam in {subject.Name} on {subject.RetakeDate:d}! Your current score: {grade.Score}");
                            }
                            else
                            {
                                _alerts.Add($"You need to retake {subject.Name}. Retake scheduled for {subject.RetakeDate:d}. Your current score: {grade.Score}");
                            }
                        }
                        else if (!subject.RetakeDate.HasValue)
                        {
                            _alerts.Add($"You need to retake {subject.Name}. Retake date not yet scheduled. Your current score: {grade.Score}");
                        }
                    }
                }

                gradeViewModels.Add(grade);
            }

            GradesGrid.ItemsSource = gradeViewModels;

            // Update alerts visibility
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
