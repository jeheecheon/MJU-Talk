
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MJU_Talk.DAL.Models;
using MJU_Talk.DAL.Models.DTOs;

namespace MJU_Talk.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<StudentUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthenticationController(UserManager<StudentUser> userManager, IConfiguration configuration)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
    {
        // Valitate the incoming request
        if (!ModelState.IsValid)
            return BadRequest();

        // Check if the email areadly exists
        var userFound = await _userManager.FindByEmailAsync(requestDto.Email);
        if (userFound is not null)
        {
            return BadRequest(new AuthResult()
            {
                Result = false,
                Errors = new List<string> {
                    "Email already exists"
                }
            });
        }

        // Create a user
        var newUser = new StudentUser()
        {
            Email = requestDto.Email,
            UserName = requestDto.Email
        };

        // Add the user and check if the result was succesful
        var isCreated = await _userManager.CreateAsync(newUser, requestDto.Password);
        if (!isCreated.Succeeded)
        {
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>() {
                    "Server error"
                },
                Result = false
            });
        }

        // Generate a new JwtToken and return it
        var token = GenerateJwtToken(newUser);
        return Ok(new AuthResult()
        {
            Result = true,
            Token = token
        });
    }

    [Route("Login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>() {
                    "Invalid payload"
                },
                Result = false
            });

        // Check if the user exists
        var userFound = await _userManager.FindByEmailAsync(loginRequest.Email);
        if (userFound is null)
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Invalid payload"
                }
            });


        // Check if the given password is correct
        var isCorrect = await _userManager.CheckPasswordAsync(userFound, loginRequest.Password);
        if (!isCorrect)
            return BadRequest(new AuthResult()
            {
                Errors = new List<string>()
                {
                    "Invalid credentials"
                },
                Result = false
            });

        var jwtToken = GenerateJwtToken(userFound);

        return Ok(new AuthResult()
        {
            Token = jwtToken,
            Result = true
        });
    }

    [NonAction]
    private string GenerateJwtToken(StudentUser user)
    {
        var key = Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!);

        // Token deescriptor
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new[]  {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString())
            }),

            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        // Convert the token to its string representation (JWT format).
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;
    }
}


/*
[HttpPost("register")]
public ActionResult<User> Register(UserDto request) {
    string passwordHash 
        = BCrypt.Net.BCrypt.HashPassword(request.Password);
    user.Username = request.Username;
    user.PasswordHash = passwordHash;

    return Ok(user);
}
[NonAction]
private string CreateToken(User user) {
    List<Claim> claims = new List<Claim>{
        new Claim(ClaimTypes.Name, user.Username)
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
        _configuration.GetSection("AppSettings:Token").Value!));
    
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

    return jwt;
}
*/
