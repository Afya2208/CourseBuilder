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
            List<Claim> claims= new List<Claim>()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                notBefore:now,
                expires: now.AddSeconds(1),
                claims: claims,
                audience: appConfiguration["JWT:Audience"],
                issuer: appConfiguration["JWT:Issuer"],
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appConfiguration["JWT:Secret"])), SecurityAlgorithms.HmacSha256)
            ); 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}