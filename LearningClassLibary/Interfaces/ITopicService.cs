using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibrary.Models;

namespace LearningClassLibrary.Interfaces
{
    public interface ITopicService
    {
        Task<List<Topic>> GetAllTopicsAsync(); // שליפת כל הנושאים
        Task<Topic?> GetTopicByIdAsync(int topicId); // שליפת נושא לפי מזהה
        Task AddTopicAsync(Topic topic); // הוספת נושא חדש
        Task UpdateTopicAsync(Topic topic); // עדכון נושא קיים
        Task DeleteTopicAsync(int topicId); // מחיקת נושא לפי מזהה
    }
}
