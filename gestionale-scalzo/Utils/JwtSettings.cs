namespace gestionale_scalzo.Utils
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
    }
}
