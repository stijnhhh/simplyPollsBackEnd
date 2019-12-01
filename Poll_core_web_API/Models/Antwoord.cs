using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class Antwoord
    {
        public long AntwoordID { get; set; }
        public string AntwoordPoll { get; set; }
        public Poll Poll { get; set; }
        public ICollection<Stem> Stemmen { get; set; }
    }
}
