using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AngloRota.Data;
using AngloRota.Data.Entities;
using AngloRota.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AngloRota.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AngloRota")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private IRepository _repository;
        private AngloRotaContext _context;
        private UserManager<User> _userManager;
        private IConfiguration _config;
        private IPasswordHasher<User> _hasher;
        private IMapper _mapper;

        public AuthController(IRepository repository, AngloRotaContext context, IMapper mapper, UserManager<User> userManager, IConfiguration config, IPasswordHasher<User> hasher)
        {
            _repository = repository;
            _context = context;
            _userManager = userManager;
            _config = config;
            _hasher = hasher;
            _mapper = mapper;
        }

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateToken([FromBody] UserModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    if (_hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                    {
                    var claims = new[]
                    {
                            new Claim (JwtRegisteredClaimNames.Sub, model.UserName),
                            new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _config["Tokens:Issuer"],
                        audience: _config["Tokens:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds
                        );
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                    }
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Exception thrown while creating JWT: {ex}");
            }
            return BadRequest("Failed to generate token");
        }
    }
}
