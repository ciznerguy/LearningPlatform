using System;
using System.ComponentModel.DataAnnotations;

namespace LearningClassLibary.Models
{
    public enum RoleIn
    {
        Student,
        Teacher,
        Admin
    }

    public class Person
    {
        // Public properties with proper validation attributes
        [Key] // Assuming this is the primary key in a database context
        public int PersonID { get; set; }

        [Required]
        [MaxLength(50)] // Adjust length as needed
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public RoleIn Role { get; set; }

        [Required]
        [MaxLength(255)] // Length of password
        public string Password { get; set; } // New field for Password

        // Constructor for initialization
        public Person()
        {
        }

        public Person(int id, string firstName, string lastName, string email, string phoneNumber, DateTime dateOfBirth, RoleIn role, string password)
        {
            PersonID = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            Role = role;
            Password = password;
        }
    }
}
