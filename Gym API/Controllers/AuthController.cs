using Gym_API.Data;
using Gym_API.DTOs;
using Gym_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Gym_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly GymContext _context;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IConfiguration _config;

        public AuthController(GymContext context, IPasswordHasher<User> hasher, IConfiguration configuration)
        {
            _context = context;
            _hasher = hasher;
            _config = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var email = request.Email.ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            var (token, expiresAt) = CreateToken(user);
            return Ok(new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                Role = user.Role
            });
        }

        private (string Token, DateTime ExpiresAt) CreateToken(User user)

        {
            var key = _config["Jwt:Key"];
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            {
                throw new InvalidOperationException("Jwt:Key is missing or shorter than 32 characters.");
            }
            if (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("Jwt:Issuer or Jwt:Audience is missing.");
            }

            var expiresAt = DateTime.UtcNow.AddMinutes(60);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = new JsonWebTokenHandler().CreateToken(descriptor);
            return (token, expiresAt);
        }

        [HttpPost("register")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            if (request.Role != Roles.Admin && request.Role != Roles.Staff)
            {
                return BadRequest();
            }
            var email = request.Email.ToLowerInvariant();
            bool r = await _context.Users.AnyAsync(x => x.Email == email);
            if(r == true)
            {
                return Conflict("Email Already Assoicated to an Account");
            }

            var user = new User
            {
                Email = email,
                PasswordHash = "",
                Role = request.Role,
            };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);

            await _context.AddAsync(user);
            await _context.SaveChangesAsync();

            RegisterResponse response = new RegisterResponse
            {
                Id =user.Id,
                Email = email,
                Role = request.Role,
            };
            return StatusCode(StatusCodes.Status201Created, response);
        }
    }
}