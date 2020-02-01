using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace DatingApp.API.Controllers
{
    // /api/auth
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }


        // client sends all user parameters in a single serialized JSON object 
        // so, use string username and string password as parameters are not suitable

        // User Object (ie. Id, Username, passwordHash, passwordSalt) is also not suitable, differet properties

        // ie. we need to use DTO - Data Trafer Object 

        // parameters are sent to the method via HTTP, asp .net core MVC automatically 
        // try to infer parameters from body or from query string or from a form,
        // you may use attribute to hint the method (ie. [FromBody])
        // as [ApiController] is in-used, [FromBody] atrribute hints can be omitted
        // public async Task<IActionResult> Register([FromBody]UserForRegisterDto userForRegisterDto)

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //convert to lowercase
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists.");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // proper way to return success message to caller is
            // return CreatedAtRoute();
            // send back the client the lcoation of newly created entity (ie. user)

            // we don't have get user method yet
            // so we simple return HTTP 201 at the time being 
            return StatusCode(201);

            //TODO: fix return CreatedAtRoute
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(
                userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value)); //read appsettings.json config

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}