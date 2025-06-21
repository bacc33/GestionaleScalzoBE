namespace gestionale_scalzo.Model.DTO
{
    public class LoginResponseDTO
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsFirstAccess { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

    }

}
