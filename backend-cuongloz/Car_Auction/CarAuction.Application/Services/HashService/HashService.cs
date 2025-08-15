using CarAuction.Application.Interfaces.HashService;

namespace CarAuction.Application.Services.HashService

{
    public class HashService : IHashService
    {
        public string ComputeMD5(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
