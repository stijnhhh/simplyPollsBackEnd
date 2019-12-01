using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Poll_core_web_API.Helpers;
using Poll_core_web_API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Poll_core_web_API.Services
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly PollContext _pollContext;
        public UserService(IOptions<AppSettings> appSettings, PollContext pollContext)
        {
            _appSettings = appSettings.Value;
            _pollContext = pollContext;
        }

        public Gebruiker Authenticate(string email, string wachtwoord)
        {
            var gebruiker = _pollContext.Gebruikers.SingleOrDefault(x => x.Email == email && x.Wachtwoord == wachtwoord);

            // return null if user not found
            if (gebruiker == null)
            {
                return null;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("GebruikersID", gebruiker.GebruikerID.ToString()),
                    new Claim("Email", gebruiker.Email),
                    new Claim("Gebruikersnaam", gebruiker.Gebruikersnaam)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            gebruiker.Token = tokenHandler.WriteToken(token);
            // remove password before returning
            gebruiker.Wachtwoord = null;

            return gebruiker;
        }
    }
}
