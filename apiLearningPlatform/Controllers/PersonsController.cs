using Microsoft.AspNetCore.Mvc;
using LearningClassLibary.Interfaces;
using LearningClassLibary.Models;

namespace LearningPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        // Get all persons
        [HttpGet]
        public async Task<IActionResult> GetAllPersons()
        {
            var persons = await _personService.GetAllPersonsAsync();
            return Ok(persons);
        }

        // Get person by ID
        [HttpGet("{PersonID}")]
        public async Task<IActionResult> GetPersonById(int PersonID)
        {
            var person = await _personService.GetPersonByIdAsync(PersonID);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        // Add a new person
        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] Person person)
        {
            if (person == null)
            {
                return BadRequest("Person is null.");
            }
            await _personService.AddPersonAsync(person);
            return CreatedAtAction(nameof(GetPersonById), new { PersonID = person.PersonID }, person);
        }

        // Update an existing person
        [HttpPut("{PersonID}")]
        public async Task<IActionResult> UpdatePerson(int PersonID, [FromBody] Person person)
        {
            if (person == null || person.PersonID != PersonID)
            {
                return BadRequest("Invalid person data.");
            }
            await _personService.UpdatePersonAsync(person);
            return NoContent();
        }

        // Delete a person
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int PersonID)
        {
            await _personService.DeletePersonAsync(PersonID);
            return NoContent();
        }
    }
}
