namespace gestionale_scalzo.Model.DTO
{
    public class RegisterRequestDTO
    {
        public string? Id { get; set; }
        public string Username { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = "Magazzino";  
    }
}
