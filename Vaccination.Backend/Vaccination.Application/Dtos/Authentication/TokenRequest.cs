﻿namespace Vaccination.Application.Dtos.Authentication
{
    public class TokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}