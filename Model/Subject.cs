using System;

namespace oop_coursework.Models
{
    public abstract class Subject
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public DateTime ExamDate { get; set; }
        public int Credits { get; set; }
        public bool IsExam { get; set; }

        public bool IsExamSoon => (ExamDate - DateTime.Now).TotalDays <= 10 && (ExamDate - DateTime.Now).TotalDays >= 0;
    }
}
