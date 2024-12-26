namespace LearningClassLibrary.Models
{
    public class Topic
    {
        public int TopicID { get; set; } // מזהה ייחודי לנושא
        public string TopicName { get; set; } // שם הנושא
        public string? Description { get; set; } // תיאור הנושא (יכול להיות NULL)
    }
}
