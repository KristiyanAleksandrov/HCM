﻿using Auth.Application.Interfaces;

namespace Auth.Infrastructure.Security
{
    public class BCryptPasswordHashService : IPasswordHashService
    {
        private const int WorkFactor = 12;
        public string Hash(string plaintext) => BCrypt.Net.BCrypt.HashPassword(plaintext, WorkFactor);
        public bool Verify(string plaintext, string hash) => BCrypt.Net.BCrypt.Verify(plaintext, hash);
    }
}
