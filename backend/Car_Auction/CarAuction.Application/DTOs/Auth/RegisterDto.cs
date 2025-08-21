namespace CarAuction.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public decimal Balance { get; set; }
    }
}
