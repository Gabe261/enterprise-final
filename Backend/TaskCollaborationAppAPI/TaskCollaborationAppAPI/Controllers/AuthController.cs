using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == request.Username && u.PasswordHash == request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        /* POST /api/auth/google == Google OAuth callback */
        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();

            if (!result.Succeeded)
            {
                return BadRequest("Google authentication failed");
            }

            var claims = result.Principal!.Identities.FirstOrDefault()!.Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email not found in Google response");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return NotFound();
            }
            
            // Generate JWT token
            var token = GenerateJwtToken(user);
            var clientUrl = _configuration["ClientUrl"];

            return Redirect($"{clientUrl}/Login/Callback?token={token}");
        }

        /* POST /api/auth/refresh == Refresh JWT Token */

        /* GET /api/auth/me == Get cuurent user info */


        // Generate JWT Token Helper Method.
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim("email", user.Email),
                new Claim("name", user.Name),
                new Claim("username", user.Username),
                new Claim("role", user.Role),
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
