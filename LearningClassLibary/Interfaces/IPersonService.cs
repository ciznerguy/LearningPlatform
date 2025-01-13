using System.Collections.Generic;
using System.Threading.Tasks;
using LearningClassLibary.Models; // Update to the correct namespace of the Person class.

namespace LearningClassLibary.Interfaces
{
    public interface IPersonService
    {
        Task<List<Person>> GetAllPersonsAsync();
        Task<Person?> GetPersonByIdAsync(int PersonID);
        Task AddPersonAsync(Person person);
        Task UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int PersonID);
    }
}