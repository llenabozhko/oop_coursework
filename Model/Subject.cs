using System;

namespace oop_coursework.Models
{
    public abstract class Subject
    {
        private int _id;
        private string _name = string.Empty;
        private DateTime? _examDate;
        private DateTime? _retakeDate;
        private int _credits;
        private bool _isExam;
        private int _semester;

        public int Id
        {
            get => _id;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Id must be positive");
                _id = value;
            }
        }

        public virtual string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Subject name cannot be empty");
                _name = value.Trim();
            }
        }

        public DateTime? ExamDate
        {
            get => _examDate;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value < DateTime.Now)
                        throw new ArgumentException("Екзамен date cannot be in the past");
                    if (_retakeDate.HasValue && value.Value >= _retakeDate.Value)
                        throw new ArgumentException("Екзамен date must be before retake date");
                }
                _examDate = value;
            }
        }

        public DateTime? RetakeDate
        {
            get => _retakeDate;
            set
            {
                _retakeDate = value;
            }
        }

        public int Credits
        {
            get => _credits;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Credits must be positive");
                if (value > 30)
                    throw new ArgumentException("Credits cannot exceed 30");
                _credits = value;
            }
        }

        public bool IsExam
        {
            get => _isExam;
            set => _isExam = value;
        }

        public int Semester
        {
            get => _semester;
            set
            {
                if (value != 1 && value != 2)
                    throw new ArgumentException("Semester must be either 1 or 2");
                _semester = value;
            }
        }

        public bool IsExamSoon => ExamDate.HasValue &&
            (ExamDate.Value - DateTime.Now).TotalDays <= 10 &&
            (ExamDate.Value - DateTime.Now).TotalDays > 0;

        public bool IsExamPassed => ExamDate.HasValue && DateTime.Now > ExamDate.Value;

        public bool NeedsRetake(double score) => score < 35 && IsExamPassed;

        public bool IsRetakeAvailable(double score) =>
            NeedsRetake(score) &&
            RetakeDate.HasValue &&
            DateTime.Now < RetakeDate.Value;

        public bool IsRetakeSoon(double score) =>
            IsRetakeAvailable(score) &&
            RetakeDate.HasValue &&
            (RetakeDate.Value - DateTime.Now).TotalDays <= 10 &&
            (RetakeDate.Value - DateTime.Now).TotalDays > 0;

        public Subject()
        {
            ExamDate = null;
            RetakeDate = null;
            Semester = (DateTime.Now.Month > 6) ? 1 : 2;
        }
    }
}
