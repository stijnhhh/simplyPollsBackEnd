using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class Stem
    {
        public long StemID { get; set; }
        public Antwoord Antwoord { get; set; }
        public Gebruiker Gebruiker { get; set; }
    }
}
