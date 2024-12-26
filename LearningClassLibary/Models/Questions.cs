using LearningClassLibrary.Models;

namespace LearningClassLibrary.Models
{
    public class Question
    {
        public int QuestionID { get; set; } // מפתח ייחודי לשאלה
        public string QuestionText { get; set; } // תוכן השאלה
        public string CorrectAnswer { get; set; } // התשובה הנכונה
        public DifficultyLevel? Difficulty { get; set; } // רמת הקושי (יכול להיות NULL)
        public string? Topic { get; set; } // נושא השאלה (יכול להיות NULL)
        public string? IncorrectAnswer1 { get; set; } // תשובה שגויה 1
        public string? IncorrectAnswer2 { get; set; } // תשובה שגויה 2
        public string? IncorrectAnswer3 { get; set; } // תשובה שגויה 3
    }
}
