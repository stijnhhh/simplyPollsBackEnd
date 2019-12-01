using System;
using System.Net;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poll_core_web_API.Models;

namespace Poll_core_web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VriendController : ControllerBase
    {
        private readonly PollContext _context;

        public VriendController(PollContext context)
        {
            _context = context;
        }

        //return een lijst van alle vrienden van een bepaalde gebruiker
        [Authorize]
        [HttpGet]
        [Route("getActieveVriend")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetActieveVriendenByGebruiker()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);
            List<Gebruiker> gebruikers = new List<Gebruiker>();
            Gebruiker gebruiker = _context.Gebruikers.Single(i => i.GebruikerID == gebruikerID);

            var listVrienden = await _context.Vrienden.Include(v => v.Verzender).Include(o => o.Ontvanger).Where(a => (a.Ontvanger.GebruikerID == gebruiker.GebruikerID || a.Verzender.GebruikerID == gebruiker.GebruikerID) && a.actief == true).ToListAsync();

            foreach (var v in listVrienden)
            {
                if(v.Ontvanger.GebruikerID == gebruikerID)
                {
                    gebruikers.Add(v.Verzender);
                }
                if (v.Verzender.GebruikerID == gebruikerID)
                {
                    gebruikers.Add(v.Ontvanger);
                }
            }

            return gebruikers;

        }

        //return de vriendschapsverzoeken van een bepaalde gebruiker
        [HttpGet]
        [Route("getVriendVerzoek")]
        public async Task<ActionResult<IEnumerable<Vriend>>> GetVriendVerzoek(int id)
        {
            Gebruiker gebruiker = _context.Gebruikers.Single(i => i.GebruikerID == id);

            return await _context.Vrienden.Include(v => v.Verzender).Include(o => o.Ontvanger).Where(a => a.Ontvanger.GebruikerID == gebruiker.GebruikerID && a.actief == false).ToListAsync();
        }

        //return vrienden om uit te nodigen voor een poll, elke gebruiker kan 1 keer uitgenodigd worden
        [Authorize]
        [HttpGet]
        [Route("getVriendVoorPoll")]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> GetVriendVoorPoll(int pollID)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            var vrienden = await _context.Vrienden.Where(g => (g.Ontvanger.GebruikerID == gebruikerID || g.Verzender.GebruikerID == gebruikerID) && g.actief == true).Include(o => o.Ontvanger).Include(v => v.Verzender).ToListAsync();
            var gebruikerlijst = new List<Gebruiker>();

            foreach(var vriend in vrienden)
            {
                if(vriend.Verzender.GebruikerID == gebruikerID)
                {
                    try
                    {
                        var pollGebruiker = await _context.PollGebruikers.Where(g => g.Gebruiker == vriend.Ontvanger && g.Poll.PollID == pollID).SingleAsync();
                    } catch
                    {
                        gebruikerlijst.Add(vriend.Ontvanger);
                    }
                }
                else
                {
                    try
                    {
                        var pollGebruiker = await _context.PollGebruikers.Where(g => g.Gebruiker == vriend.Verzender && g.Poll.PollID == pollID).SingleAsync();
                    }
                    catch
                    {
                        gebruikerlijst.Add(vriend.Verzender);
                    }
                }
            }

            return gebruikerlijst;
        }

        //return het aantal vriendschapsverzoeken van een bepaalde gebruiker
        [Authorize]
        [HttpGet]
        [Route("getcount")]
        public async Task<ActionResult<int>> GetCount()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            var number = await _context.Vrienden.Where(v => v.Ontvanger.GebruikerID == gebruikerID && v.actief == false).CountAsync();
            if (number == 0)
            {

            }
            return number;
        }

        //Verkrijg alle vrienden
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vriend>>> Getvrienden()
        {
            return await _context.Vrienden.Include(v => v.Verzender).Include(o => o.Ontvanger).ToListAsync();
        }

        // GET: api/Vriend/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vriend>> GetVriend(long id)
        {
            var vriend = await _context.Vrienden.FindAsync(id);

            if (vriend == null)
            {
                return NotFound();
            }

            return vriend;
        }

        // PUT: api/Vriend/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVriend(long id, Vriend vriend)
        {
            if (id != vriend.VriendID)
            {
                return BadRequest();
            }

            _context.Entry(vriend).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VriendExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        //Maak een vriend aan en controleer of het vriendobject al bestaat
        [HttpPost]
        public async Task<ActionResult<Vriend>> PostVriend(Gebruiker gebruiker)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);
            Gebruiker gebruiker2 = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID);

            Vriend vriend = new Vriend();

            try
            {
                vriend = await _context.Vrienden.Where(o => (o.Ontvanger.GebruikerID == gebruiker.GebruikerID && o.Verzender.GebruikerID == gebruiker2.GebruikerID) || (o.Ontvanger.GebruikerID == gebruiker2.GebruikerID && o.Verzender.GebruikerID == gebruiker.GebruikerID)).SingleAsync();

                return NoContent();
            } catch
            {
                vriend.Verzender = gebruiker2;
                vriend.Ontvanger = gebruiker;
                vriend.actief = false;
                vriend.VriendID = 0;

                _context.Entry(vriend.Ontvanger).State = EntityState.Modified;
                _context.Entry(vriend.Verzender).State = EntityState.Modified;

                _context.Vrienden.Add(vriend);
                await _context.SaveChangesAsync();

                if(vriend.Ontvanger.Gebruikersnaam == null)
                {
                    verstuurMail(vriend.Ontvanger.Email, vriend.Verzender.Gebruikersnaam);
                }

                return vriend;
            }
        }

        // DELETE: api/Vriend/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Vriend>> DeleteVriend(long id)
        {
            var vriend = await _context.Vrienden.FindAsync(id);
            if (vriend == null)
            {
                return NotFound();
            }

            _context.Vrienden.Remove(vriend);
            await _context.SaveChangesAsync();

            return vriend;
        }

        //Verwijder een vriend met behulp van de id's van 2 gebruikers
        [Authorize]
        [HttpDelete]
        [Route("verwijderVriendById")]
        public async Task<ActionResult<Vriend>> DeleteVriendById(long id)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            var vriend = await _context.Vrienden.Where(o => (o.Ontvanger.GebruikerID == gebruikerID && o.Verzender.GebruikerID == id) || (o.Ontvanger.GebruikerID == id && o.Verzender.GebruikerID == gebruikerID)).SingleAsync();
            if (vriend == null)
            {
                return NotFound();
            }

            _context.Vrienden.Remove(vriend);
            await _context.SaveChangesAsync();

            return vriend;
        }

        private bool VriendExists(long id)
        {
            return _context.Vrienden.Any(e => e.VriendID == id);
        }

        //stel mail op en verstuur deze
        private void verstuurMail(string email, string gebruikersnaam)
        {
            MailAddress to = new MailAddress(email);
            MailAddress from = new MailAddress("piotr@mailtrap.io");

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Simply Polls uitnodiging";
            message.Body = "Je bent uitgenodigd door " + gebruikersnaam + "\n\n\n" +
                "Klik op de onderstaande link om een account aan te maken met dit emailadres:\n\n" +
                "http://localhost:4200/registratie \n\n" +
                "Tot snel,\n" +
                "Simply Polls";

            SmtpClient client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("57d5298d3b4bad", "53640843fcfb34"),
                EnableSsl = true
            };

            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
