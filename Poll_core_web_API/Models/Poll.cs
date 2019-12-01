using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class Poll
    {
        public long PollID { get; set; }
        public string Naam { get; set; }
        public ICollection<Antwoord> Antwoorden { get; set; }
        public ICollection<PollGebruiker> PollGebruikers { get; set; }
    }
}
