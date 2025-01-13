using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibrary.Models;

namespace LearningClassLibrary.Interfaces
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAllQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int questionId);

        Task<int> GetQuestionsCountAsync();
    }
}
