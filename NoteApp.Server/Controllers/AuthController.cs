using Microsoft.AspNetCore.Mvc;
using NoteApp.Server.Data;
using NoteApp.Server.DTOs;
using NoteApp.Server.Models;
using NoteApp.Server.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace NoteApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;


        public AuthController(AppDbContext context, IAuthService authService, IConfiguration config)
        {
            _context = context;
            _authService = authService;
            _config = config;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username already exists");


            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
                PasswordSalt = hmac.Key
            };


            _context.Users.Add(user);
            await _context.SaveChangesAsync();


            return Ok(new { Message = "Registration successful" });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO model)
        {
            var user = await _authService.Authenticate(model.Username, model.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");


            var token = _authService.GenerateJwtToken(user);
            var refreshToken = _authService.GenerateRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays"));
            await _context.SaveChangesAsync();


            return Ok(new
            {
                accessToken = token,
                refreshToken,
                username = user.Username,
                userId = user.Id
            });
        }


        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO request)
        {
            var principal = _authService.GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);


            if (!int.TryParse(userId, out var userIdInt))
                return BadRequest("Invalid user ID in token");


            var user = await _context.Users.FindAsync(userIdInt);
            if (user == null || user.RefreshToken != request.RefreshToken ||
                user.RefreshTokenExpiry <= DateTime.UtcNow)
                return Unauthorized("Invalid refresh token");


            var newToken = _authService.GenerateJwtToken(user);
            var newRefreshToken = _authService.GenerateRefreshToken();


            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshTokenExpirationDays"));
            await _context.SaveChangesAsync();


            return Ok(new
            {
                accessToken = newToken,
                refreshToken = newRefreshToken
            });
        }
    }
}

