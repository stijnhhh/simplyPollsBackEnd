using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poll_core_web_API.Models
{
    public class DBInitializer
    {
        public static void Initialize(PollContext context)
        {
            context.Database.EnsureCreated();
            
            if(context.Gebruikers.Any() || context.PollGebruikers.Any() || context.Polls.Any() || context.Antwoorden.Any())
            {
                return;
            }

            context.Gebruikers.AddRange(
                new Gebruiker
                {
                    Wachtwoord = "Stijnh",
                    Email = "stijnhaerkens@telenet.be",
                    Gebruikersnaam = "stijnhhh"
                },
                new Gebruiker
                {
                    Wachtwoord = "vriend",
                    Email = "deyrh123@gmail.com",
                    Gebruikersnaam = "deyrh"
                },
                new Gebruiker
                {
                    Wachtwoord="aaaaa",
                    Email = "aaa@aaa.aaa",
                    Gebruikersnaam="aaaaa"
                });
            context.SaveChanges();

            context.Polls.AddRange(
                new Poll
                {
                    Naam = "eerste polltest"
                }
                );
            context.SaveChanges();

            context.PollGebruikers.AddRange(
                new PollGebruiker
                {
                    Gebruiker = context.Gebruikers.FirstOrDefault(),
                    Poll = context.Polls.FirstOrDefault(),
                    Admin = context.Gebruikers.FirstOrDefault(),
                    Actief = true
                },
                new PollGebruiker
                {
                    Gebruiker = context.Gebruikers.Single(g => g.GebruikerID == 2),
                    Poll = context.Polls.FirstOrDefault(),
                    Admin = context.Gebruikers.FirstOrDefault(),
                    Actief = false
                }
                );
            context.SaveChanges();

            context.Antwoorden.AddRange(
                new Antwoord
                {
                    AntwoordPoll = "1",
                    Poll = context.Polls.Single(a => a.PollID == 1)
                },
                new Antwoord
                {
                    AntwoordPoll = "2",
                    Poll = context.Polls.Single(a => a.PollID == 1)
                },
                new Antwoord
                {
                    AntwoordPoll = "3",
                    Poll = context.Polls.Single(a => a.PollID == 1)
                }
                );
            context.SaveChanges();

            context.Stemmen.AddRange(
                new Stem
                {
                    Gebruiker = context.Gebruikers.Single(g => g.GebruikerID == 1),
                    Antwoord = context.Antwoorden.Single(a => a.AntwoordID == 1)
                }
                );
            context.SaveChanges();

            context.Vrienden.AddRange(
                new Vriend
                {
                    Verzender = context.Gebruikers.Single(v => v.GebruikerID == 1),
                    Ontvanger = context.Gebruikers.Single(o => o.GebruikerID == 3),
                    actief = false
                }
                );
            context.SaveChanges();
        }
    }
}
