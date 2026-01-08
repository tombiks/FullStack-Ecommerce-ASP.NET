using AppBusiness.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminMvc.Helpers
{    
    public class ClaimHelper
    {
        private readonly IConfiguration _config;

        public ClaimHelper(IConfiguration config)
        {
            _config = config;
        }

        public string ClaimsIdendityHelper(UserCookieDTO userCookieDTO)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userCookieDTO.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, userCookieDTO.Name!),
                new Claim("Surname", userCookieDTO.Surname!),
                new Claim(JwtRegisteredClaimNames.Email, userCookieDTO.Email!),
                new Claim(ClaimTypes.Role, userCookieDTO.Role!),
            };

            var jwtSecret = _config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret configuration is missing");
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var tokenOptions = new JwtSecurityToken(
                issuer: "AdminMVC",
                audience: "MVC",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256));


            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return tokenString;
        }

        public int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return 0;
        }
    }
}
