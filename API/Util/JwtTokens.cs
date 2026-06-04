using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Models.Entities;

namespace API.Util
{
    public static class JwtTokens
    {
        public static string GenerateToken(IConfiguration appConfiguration, User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };
           
            var identity = new ClaimsIdentity(claims, "JWT", ClaimTypes.Email, ClaimTypes.Role);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                notBefore:now,
                expires: now.AddHours(12),
                claims: identity.Claims,
                audience: appConfiguration["JWT:Audience"],
                issuer: appConfiguration["JWT:Issuer"],
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfiguration["JWT:Secret"])), SecurityAlgorithms.HmacSha256)
            ); 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string GenerateVideoToken(IConfiguration appConfiguration, string email, long videoId)
        {
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                notBefore:now,
                expires: now.AddMinutes(3),
                claims: new []
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim("VID", videoId.ToString())
                },
                audience: appConfiguration["JWT:Audience"],
                issuer: appConfiguration["JWT:Issuer"],
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfiguration["JWT:Secret"])), 
                    SecurityAlgorithms.HmacSha256)
            ); 
            var tokenText =  new JwtSecurityTokenHandler().WriteToken(token);
            return Uri.EscapeDataString(tokenText);
        }
    }
}