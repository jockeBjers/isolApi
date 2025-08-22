namespace IsolCore.Models

{
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; }
    }
}