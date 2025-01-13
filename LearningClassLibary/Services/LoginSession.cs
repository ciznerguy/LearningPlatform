namespace LearningClassLibrary.Services
{
    public class LoginSession
    {
        public int PersonID { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Role { get; private set; } = string.Empty;

        public void SetLoginDetails(int personId, string name, string email, string role)
        {
            PersonID = personId;
            Name = name;
            Email = email;
            Role = role;
        }

        public void ClearSession()
        {
            PersonID = 0;
            Name = string.Empty;
            Email = string.Empty;
            Role = string.Empty;
        }
    }
}
