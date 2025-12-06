using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskCollaborationAppAPI.Data;
using TaskCollaborationAppAPI.Models;

namespace TaskCollaborationAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /* POST /api/auth/register == User registration */

        /* POST /api/auth/login == Login with username/passowrd */
        [HttpPost("Login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.PasswordHash == request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            var token = GenerateToken(user);
            return Ok(new { token });
        }

        /* POST /api/auth/google == Google OAuth callback */

        /* POST /api/auth/refresh == Refresh JWT Token */

        /* GET /api/auth/me == Get cuurent user info */


        // Generate JWT Token Helper Method.
        private Object GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var secret = _configuration["JwtSettings:Secret"];
            var expiresDays = int.Parse(_configuration["JwtSettings:ExpiresDays"]);

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(expiresDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
