using System;
using System.Collections.Generic;
using System.Linq;
using oop_coursework.Models;

namespace oop_coursework.Services
{
    public class StudentService
    {
        private readonly DataService _dataService;

        public StudentService(DataService dataService)
        {
            _dataService = dataService;
        }

        public List<string> GenerateAlerts(Student student, int semester)
        {
            var alerts = new List<string>();
            var subjects = _dataService.GetSubjects();
            var grades = _dataService.GetGrades()
                .Where(g => g.StudentId == student.Id && g.Semester == semester)
                .ToList();

            foreach (var subject in subjects)
            {
                var grade = grades.FirstOrDefault(g => g.SubjectId == subject.Id) ??
                    new Grade
                    {
                        Subject = subject,
                        Score = 0,
                        Semester = semester
                    };

                if (subject.IsExam)
                {
                    if (subject.IsExamSoon && grade.Score == 0)
                    {
                        alerts.Add($"Upcoming exam in {subject.Name} on {subject.ExamDate:d}!");
                    }
                    else if (subject.NeedsRetake(grade.Score))
                    {
                        if (subject.IsRetakeAvailable(grade.Score))
                        {
                            if (subject.IsRetakeSoon(grade.Score))
                            {
                                alerts.Add($"URGENT: Retake exam in {subject.Name} on {subject.RetakeDate:d}! Your current score: {grade.Score}");
                            }
                            else
                            {
                                alerts.Add($"You need to retake {subject.Name}. Retake scheduled for {subject.RetakeDate:d}. Your current score: {grade.Score}");
                            }
                        }
                        else if (!subject.RetakeDate.HasValue)
                        {
                            alerts.Add($"You need to retake {subject.Name}. Retake date not yet scheduled. Your current score: {grade.Score}");
                        }
                    }
                }
            }

            return alerts;
        }
    }
}
