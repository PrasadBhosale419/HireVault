using System;
using System.Collections.Generic;

namespace HireVault.Web.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresIn { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public Dictionary<string, string> UserAttributes { get; set; } = new Dictionary<string, string>();

        public static AuthResponse SuccessResponse(string accessToken, string idToken, string refreshToken, DateTime expiresIn, Dictionary<string, string> userAttributes = null)
        {
            return new AuthResponse
            {
                Success = true,
                AccessToken = accessToken,
                IdToken = idToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn,
                UserAttributes = userAttributes ?? new Dictionary<string, string>()
            };
        }

        public static AuthResponse ErrorResponse(params string[] errors)
        {
            return new AuthResponse
            {
                Success = false,
                Errors = new List<string>(errors)
            };
        }
    }
}
