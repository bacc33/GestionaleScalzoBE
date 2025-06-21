namespace gestionale_scalzo.Model.DTO
{
    public class ApplicationUserDTO
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Enabled { get; set; }
    }
}

