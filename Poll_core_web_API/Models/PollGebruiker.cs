using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class PollGebruiker
    {
        public long PollGebruikerID { get; set; }
        public Gebruiker Admin { get; set; }
        public Boolean Actief { get; set; }
        public Poll Poll { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}
