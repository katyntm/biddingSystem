namespace CarAuction.Application.DTOs.Auth
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }
        public string UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
