using System;

namespace oop_coursework.Models
{
    public abstract class Subject
    {
        public int Id { get; set; }
        public required virtual string Name { get; set; }
        public DateTime? ExamDate { get; set; }
        public DateTime? RetakeDate { get; set; }
        public int Credits { get; set; }
        public bool IsExam { get; set; }
        public int Semester { get; set; }

        public bool IsExamSoon => ExamDate.HasValue && (ExamDate.Value - DateTime.Now).TotalDays <= 10 && (ExamDate.Value - DateTime.Now).TotalDays >= 0;
        public bool IsRetakeSoon => RetakeDate.HasValue && (RetakeDate.Value - DateTime.Now).TotalDays <= 10 && (RetakeDate.Value - DateTime.Now).TotalDays >= 0;

        public bool CanSetRetakeDate => ExamDate.HasValue && DateTime.Now >= ExamDate.Value;

        public Subject()
        {
            ExamDate = null;
            RetakeDate = null;
            Semester = (DateTime.Now.Month > 6) ? 1 : 2;
        }
    }
}
