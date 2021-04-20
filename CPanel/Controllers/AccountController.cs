using CPanel.MqttServer;
using CPanel.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using USQLCSharp.DataAccess;
using USQLCSharp.Models;

namespace CPanel.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        [HttpPost("token")]
        public IActionResult Token(string username, string password)
        {
            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };
            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            using var _db = new PeopleContext();
            Person person = _db.Person.FirstOrDefault(x => x.Login == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role),
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователя не найдено
            return null;
        }
        [HttpPost("Register")]
        public IActionResult Register(string login, string password, string firstName, string lastName)
        {
            using var _db = new PeopleContext();
            var persons = _db.Person.FirstOrDefault(x => x.Login == login);
            if(persons != null) return BadRequest(new { errorText = "Email already registered" });
            var user = new Person
            {
                Login = login,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Role = "user"
            };
            _db.Person.Add(user);
            _db.SaveChanges();
            return Ok();
        }
    }
}
    