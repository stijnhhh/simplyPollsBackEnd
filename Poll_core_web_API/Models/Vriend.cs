using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class Vriend
    {
        public long VriendID { get; set; }
        public Gebruiker Verzender { get; set; }
        public Gebruiker Ontvanger { get; set; }
        public Boolean actief { get; set; }
    }
}
