using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TaskCollaborationAppAPI.Data;
using TaskCollaborationAppAPI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

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
        [HttpPost("refresh")]
        public ActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

            // Validate the token without checking expiration
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = false
            };

            var principal = tokenHandler.ValidateToken(request.Token, validationParameters, out var validatedToken);

            // Get user ID from claims
            var userIdClaim = principal.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            // Get user from database
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Generate new token using existing method
            var newToken = GenerateJwtToken(user);
            return Ok(new { token = newToken });
        }

        /* GET /api/auth/me == Get current user info */
        [HttpGet("me")]
        public ActionResult GetCurrentUser()
        {
            // Get token from Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("No token provided");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _configuration["JwtSettings:Secret"];
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));

            // Validate token
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Get user ID from claims
            var userIdClaim = principal.FindFirst("id");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid token");
            }

            // Get user from database
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Return user info (without password hash)
            return Ok(new
            {
                id = user.Id,
                username = user.Username,
                email = user.Email,
                name = user.Name,
                role = user.Role
            });
        }


        // Generate JWT Token Helper Method.
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.Name),
                new Claim("username", user.Username),
                new Claim("role", user.Role),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var secret = _configuration["JwtSettings:Secret"];
            var expiresDays = int.Parse(_configuration["JwtSettings:ExpiresDays"]);
            var audience = _configuration["JwtSettings:Audience"];
            var issuer = _configuration["JwtSettings:Issuer"];

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                audience: audience,
                issuer: issuer,
                claims: claims,
                expires: DateTime.Now.AddDays(expiresDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
