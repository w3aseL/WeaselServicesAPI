namespace WeaselServicesAPI.Configuration
{
    public class JWTSettings
    {
        public string Secret { get; set; }
        public int AccessTokenExpirationTime { get; set; }
        public int RefreshTokenExpirationTime { get; set; }
    }
}
