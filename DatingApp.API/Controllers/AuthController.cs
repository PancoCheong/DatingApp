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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DatingApp.API.Controllers
{
    // /api/auth
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(IConfiguration config, IMapper mapper,
        UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
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
            //ASP Identity take care of both unique username and duplicated user check
            // userForRegisterDto.Username = userForRegisterDto.Username.ToLower();
            // if (await _repo.UserExists(userForRegisterDto.Username))
            //     return BadRequest("Username already exists.");

            var userToCreate = _mapper.Map<User>(userForRegisterDto); /* use Auto Mapper */

            // use Identity user Manager instead of our Repo
            var result = await _userManager.CreateAsync(userToCreate, userForRegisterDto.Password);
            //var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            var userToReturn = _mapper.Map<UserForDetailedDto>(userToCreate);
            if (result.Succeeded)
            {
                return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //no longer user this login method
            //var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password); 
            var user = await _userManager.FindByNameAsync(userForLoginDto.Username);
            var result = await _signInManager.CheckPasswordSignInAsync(
                user, userForLoginDto.Password, false); // false - not lock out user when attempting to sign in  

            //if (userFromRepo == null) return Unauthorized();
            if (result.Succeeded)
            {
                var appUser = _mapper.Map<UserForListDto>(user);

                return Ok(new
                {
                    token = GenerateJwtToken(user),
                    user = appUser
                });
            }

            return Unauthorized();
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            // Generate the JWT token
            // building a list of claims for user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // set the symmetric key for encryption
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config.GetSection("AppSettings:Token").Value)); //read appsettings.json config

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            // create the JWT token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}