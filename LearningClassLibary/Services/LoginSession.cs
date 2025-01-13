namespace LearningClassLibrary.Services
{
    public class LoginSession
    {
        // ערכי ברירת מחדל עבור אורח
        public int PersonID { get; private set; } = 0;
        public string Name { get; private set; } = "Guest";
        public string Email { get; private set; } = "guest@site.com";
        public string Role { get; private set; } = "Guest";

        public void SetLoginDetails(int personId, string name, string email, string role)
        {
            PersonID = personId;
            Name = name;
            Email = email;
            Role = role;
        }

        public void ClearSession()
        {
            // חזרה לערכי ברירת מחדל של אורח
            PersonID = 0;
            Name = "Guest";
            Email = "guest@site.com";
            Role = "Guest";
        }

        // פונקציה לבדוק אם המשתמש הוא אורח
        public bool IsGuest()
        {
            return Role == "Guest";
        }
    }
}
