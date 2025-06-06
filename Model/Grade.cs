using System;

namespace oop_coursework.Models
{
    public class Grade
    {
        private int _id;
        private int _studentId;
        private int _subjectId;
        private double _score;
        private double? _retakeScore;
        private Subject _subject;
        private int _semester;
        private DateTime _gradeDate;
        private DateTime? _retakeDate;

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

        public int StudentId
        {
            get => _studentId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("StudentId must be positive");
                _studentId = value;
            }
        }

        public int SubjectId
        {
            get => _subjectId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("SubjectId must be positive");
                _subjectId = value;
            }
        }

        public double Score
        {
            get => _score;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentException("Score must be between 0 and 100");
                _score = value;
            }
        }

        public double? RetakeScore
        {
            get => _retakeScore;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value < 0 || value.Value > 100)
                        throw new ArgumentException("Retake score must be between 0 and 100");
                    if (!CanRetake)
                        throw new InvalidOperationException("Cannot set retake score when retake is not allowed");
                }
                _retakeScore = value;
            }
        }

        public Subject Subject
        {
            get => _subject;
            set => _subject = value ?? throw new ArgumentNullException(nameof(value), "Subject cannot be null");
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

        public DateTime GradeDate
        {
            get => _gradeDate;
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Grade date cannot be in the future");
                _gradeDate = value;
            }
        }

        public DateTime? RetakeDate
        {
            get => _retakeDate;
            set
            {
                if (value.HasValue)
                {
                    if (value.Value <= GradeDate)
                        throw new ArgumentException("Retake date must be after grade date");
                    if (value.Value > DateTime.Now)
                        throw new ArgumentException("Retake date cannot be in the future");
                }
                _retakeDate = value;
            }
        }

        public double FinalScore
        {
            get
            {
                if (Score >= 60) return Score;
                return RetakeScore ?? Score;
            }
        }

        public string Status
        {
            get
            {
                if (Score == 0) return "Не оцінено";
                if (Score >= 60) return "Здано";
                if (RetakeScore.HasValue)
                {
                    return RetakeScore >= 60 ? "Здано (Перездача)" : "Не здано";
                }
                if (Subject.RetakeDate.HasValue && DateTime.Now < Subject.RetakeDate.Value)
                {
                    return "На перездачу";
                }
                return "Не здано";
            }
        }

        public bool CanRetake => Score > 0 && Score < 60 && !RetakeScore.HasValue;

        public bool NeedsRetake => Score > 0 && Score < 60 && !RetakeScore.HasValue && Subject.RetakeDate.HasValue;

        public Grade()
        {
            GradeDate = DateTime.Now;
        }
    }
}
