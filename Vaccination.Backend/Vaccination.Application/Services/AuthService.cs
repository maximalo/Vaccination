using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Vaccination.Application.Dtos.Authentication;
using Vaccination.Application.Interfaces;
using Vaccination.Application.Shared;
using Vaccination.Domain.Entities;
using Vaccination.Domain.Interfaces;

namespace Vaccination.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager, IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            User? user = await _userManager.FindByEmailAsync(loginRequest.Email);

            if (user is null)
            {
                return new LoginResponse()
                {
                    IsSucceed = false,
                    Message = "User does not exists"
                };
            }
            bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

            if (!isPasswordCorrect)
            {
                return new LoginResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid credentials"
                };
            }

            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> authClaims = new List<Claim>
            {
               // new Claim(ClaimTypes.Email, user.Email),
               // new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("email", user.Email),
                new Claim("userid", user.Id),
                new Claim("jwtid", Guid.NewGuid().ToString()),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
            };

            foreach (string userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            JwtSecurityToken token = GenerateNewJsonWebToken(authClaims);
            string refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["JWT:TokenValidity"], out int refreshTokenValidityInDays);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

            IdentityResult updateResult = await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            if (!updateResult.Succeeded)
            {
                return new LoginResponse()
                {
                    IsSucceed = false,
                    Message = "Failed to update user with new tokens"
                };
            }

            return new LoginResponse()
            {
                IsSucceed = true,
                Message = "Login success",
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            };
        }

        public async Task<TokenResponse> RefreshTokenAsync(TokenRequest tokenRequest)
        {
            if (tokenRequest is null)
            {
                return new TokenResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid client request"
                };
            }

            string? accessToken = tokenRequest.AccessToken;
            string? refreshToken = tokenRequest.RefreshToken;

            ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal is null)
            {
                return new TokenResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid access token or refresh token"
                };
            }

            if (principal.Identity is null)
            {
                return new TokenResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid access token or refresh token"
                };
            }

            string email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty; ;

            User? user = await _userManager.FindByEmailAsync(email);

            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return new TokenResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid access token or refresh token"
                };
            }

            JwtSecurityToken newAccessToken = GenerateNewJsonWebToken(principal.Claims.ToList());
            string newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return new TokenResponse()
            {
                IsSucceed = true,
                Message = "token refreshed",
                Token = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken,
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            User? isExistsUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (isExistsUser != null)
            {
                return new RegisterResponse()
                {
                    IsSucceed = false,
                    Message = "User already exsists"
                };
            }
            User newUser = new()
            {
                Email = registerRequest.Email,
                UserName = registerRequest.Email,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            IdentityResult createUserResult = await _userManager.CreateAsync(newUser, registerRequest.Password);
            if (!createUserResult.Succeeded)
            {
                string errorString = "UserCreation Failed because : ";
                foreach (IdentityError error in createUserResult.Errors)
                {
                    errorString += " # " + error.Description;
                }
                return new RegisterResponse()
                {
                    IsSucceed = false,
                    Message = errorString
                };
            }
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.READ);
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.WRITE);
            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.DELETE);
            return new RegisterResponse()
            {
                IsSucceed = true,
                Message = "User created successfully"
            };
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[64];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken GenerateNewJsonWebToken(List<Claim> claims)
        {
            SymmetricSecurityKey authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            JwtSecurityToken tokenObject = new JwtSecurityToken(
                     issuer: _configuration["JWT:ValidIssuer"],
                     audience: _configuration["JWT:ValidAudience"],
                     expires: DateTime.Now.AddHours(1),
                     claims: claims,
                     signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
                 );

            return tokenObject;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"])),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}