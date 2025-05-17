using System;

namespace Auth.Application.ResponseModels
{
    public class AuthResponse
    {
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
