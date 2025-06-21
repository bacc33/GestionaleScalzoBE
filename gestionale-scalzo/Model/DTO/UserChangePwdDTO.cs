namespace gestionale_scalzo.Model.DTO
{
    public class UserChangePwdDTO
    {
        public int UserId { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
