using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string RefreshAccessToken(string refreshToken);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly double _expirationDays;

    public TokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _secretKey = _configuration["JwtSettings:SecretKey"];
        _issuer = _configuration["JwtSettings:Issuer"];
        _audience = _configuration["JwtSettings:Audience"];
        _expirationDays = Convert.ToDouble(_configuration["JwtSettings:ExpirationDays"]);
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new List<Claim>
        {
            new Claim("sub", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
            // Add role claims
        }),
            Expires = DateTime.UtcNow.AddDays(_expirationDays),
            Issuer = _issuer,
            Audience = _audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        // Add user roles as claims
        var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
        foreach (var role in roles)
        {
            tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public string RefreshAccessToken(string refreshToken)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        if (user == null)
        {
            // Refresh token not found in the database
            return null;
        }

        var newAccessToken = GenerateAccessToken(user);
        var newRefreshToken = GenerateRefreshToken();

        // Update the user's refresh token in the database
        user.RefreshToken = newRefreshToken;
        _userManager.UpdateAsync(user).GetAwaiter().GetResult();

        return newAccessToken;
    }
}
