using Poll_core_web_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Services
{
    public interface IUserService
    {
        Gebruiker Authenticate(string gebruikersnaam, string wachtwoord);
    }
}
