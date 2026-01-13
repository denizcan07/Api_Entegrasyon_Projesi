using ApiEntegrasyon.Entity;
using ApiEntegrasyon.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiEntegrasyon.Helpers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsersRepository _usersRepo;

    public AuthController(IUsersRepository usersRepo)
    {
        _usersRepo = usersRepo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string username, string password)
    {
        string hashpass = PasswordHelper.Hash(password);
        var user = await _usersRepo.GetByUsernameAndPassword(username, hashpass);

        if (user == null)
            return Unauthorized("Kullanıcı adı veya şifre yanlış.");

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "Admin") 
        };

        var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
        );

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];


        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}

