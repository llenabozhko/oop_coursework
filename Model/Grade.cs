using System;

namespace oop_coursework.Models
{
    public class Grade
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public double Score { get; set; }
        public double? RetakeScore { get; set; }
        public required Subject Subject { get; set; }
        public int Semester { get; set; }
        public DateTime GradeDate { get; set; }
        public DateTime? RetakeDate { get; set; }

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
                if (Score == 0) return "Not Graded";
                if (Score >= 60) return "Passed";
                if (RetakeScore.HasValue)
                {
                    return RetakeScore >= 60 ? "Passed (Retake)" : "Failed";
                }
                if (Subject.RetakeDate.HasValue && DateTime.Now < Subject.RetakeDate.Value)
                {
                    return "Retake Available";
                }
                return "Failed";
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
