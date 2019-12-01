using System;
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
    public class PollGebruikerController : ControllerBase
    {
        private readonly PollContext _context;

        public PollGebruikerController(PollContext context)
        {
            _context = context;
        }

        // GET: api/PollGebruiker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PollGebruiker>>> GetpollGebruikers()
        {
            return await _context.PollGebruikers.ToListAsync();
        }

        [Authorize]
        [HttpGet]
        [Route("getpolls")]
        public async Task<ActionResult<IEnumerable<PollGebruiker>>> GetPolls()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            return await _context.PollGebruikers
                .Include(a => a.Admin)
                .Include(p => p.Poll)
                    .ThenInclude(a => a.Antwoorden)
                .Include(p => p.Gebruiker)
                    .ThenInclude(s => s.Stemmen).Where(g => g.Gebruiker.GebruikerID == gebruikerID && g.Actief == true && g.Admin.GebruikerID != gebruikerID).ToListAsync();
        }

        [Authorize]
        [HttpGet]
        [Route("geteigenpolls")]
        public async Task<ActionResult<IEnumerable<PollGebruiker>>> GetEigenPolls()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            return await _context.PollGebruikers
                .Include(a => a.Admin)
                .Include(p => p.Poll)
                    .ThenInclude(a => a.Antwoorden)
                .Include(p => p.Gebruiker)
                    .ThenInclude(s => s.Stemmen).Where(g => g.Gebruiker.GebruikerID == gebruikerID && g.Actief == true && g.Admin.GebruikerID == gebruikerID).ToListAsync();
        }

        [Authorize]
        [HttpGet]
        [Route("getpollverzoeken")]
        public async Task<ActionResult<IEnumerable<PollGebruiker>>> GetpollVerzoeken()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            return await _context.PollGebruikers
                .Include(a => a.Admin)
                .Include(p => p.Poll)
                    .ThenInclude(a => a.Antwoorden)
                .Include(p => p.Gebruiker)
                    .ThenInclude(s => s.Stemmen).Where(g => g.Gebruiker.GebruikerID == gebruikerID && g.Actief == false).ToListAsync();
        }

        // GET: api/PollGebruiker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PollGebruiker>> GetPollGebruiker(long id)
        {
            var pollGebruiker = await _context.PollGebruikers.FindAsync(id);

            if (pollGebruiker == null)
            {
                return NotFound();
            }

            return pollGebruiker;
        }

        // GET: api/PollGebruiker/5
        [Authorize]
        [HttpGet]
        [Route("zoekPoll")]
        public async Task<ActionResult<PollGebruiker>> GetPoll(int pollID)
        {
            PollGebruiker pollGebruiker =  await _context.PollGebruikers
                .Include(a => a.Admin)
                .Include(p => p.Poll)
                    .ThenInclude(a => a.Antwoorden)
                        .ThenInclude(s => s.Stemmen)
                            .ThenInclude(g=> g.Gebruiker).Where(i => i.Poll.PollID == pollID).FirstOrDefaultAsync();

            return pollGebruiker;
        }

       // PUT: api/PollGebruiker/5
       [HttpPut("{id}")]
        public async Task<IActionResult> PutPollGebruiker(long id, PollGebruiker pollGebruiker)
        {
            if (id != pollGebruiker.PollGebruikerID)
            {
                return BadRequest();
            }

            _context.Entry(pollGebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollGebruikerExists(id))
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

        // DELETE: api/PollGebruiker/5
        [HttpPut]
        [Route("accepteerverzoek")]
        public async Task<ActionResult<PollGebruiker>> AccepteerVerzoekPollGebruiker(PollGebruiker pollGebruiker)
        {
            pollGebruiker.Actief = true;

            _context.Entry(pollGebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/PollGebruiker
        [HttpPost]
        public async Task<ActionResult<PollGebruiker>> PostPollGebruiker(PollGebruiker pollGebruiker)
        {
            _context.PollGebruikers.Add(pollGebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollGebruiker", new { id = pollGebruiker.PollGebruikerID }, pollGebruiker);
        }

        // POST: api/PollGebruiker
        [Authorize]
        [HttpPost]
        [Route("nodigvrienduit")]
        public async Task<ActionResult<PollGebruiker>> NodigVriendUit(int pollID, Gebruiker gebruiker)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            PollGebruiker pollGebruiker = new PollGebruiker();

            pollGebruiker.Admin = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID);
            pollGebruiker.Actief = false;
            pollGebruiker.Gebruiker = gebruiker;
            pollGebruiker.Poll = await _context.Polls.SingleAsync(p => p.PollID == pollID);
            pollGebruiker.PollGebruikerID = 0;

            _context.Entry(pollGebruiker.Gebruiker).State = EntityState.Modified;
            _context.Entry(pollGebruiker.Admin).State = EntityState.Modified;
            _context.Entry(pollGebruiker.Poll).State = EntityState.Modified;

            _context.PollGebruikers.Add(pollGebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPollGebruiker", new { id = pollGebruiker.PollGebruikerID }, pollGebruiker);
        }

        // DELETE: api/PollGebruiker/5
        [HttpDelete]
        [Route("verwijderverzoek")]
        public async Task<ActionResult<PollGebruiker>> DeleteVerzoekPollGebruiker(long id)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            var pollGebruiker = await _context.PollGebruikers.Where(g => g.Gebruiker.GebruikerID == gebruikerID).SingleAsync(i => i.PollGebruikerID == id);
            if (pollGebruiker == null)
            {
                return NotFound();
            }

            _context.PollGebruikers.Remove(pollGebruiker);
            await _context.SaveChangesAsync();

            return pollGebruiker;
        }

        // DELETE: api/PollGebruiker/5
        [Authorize]
        [HttpDelete]
        [Route("verwijderpoll")]
        public async Task<ActionResult<Poll>> DeletePoll(long id)
        {
            var poll = await _context.Polls
                .Include(a => a.Antwoorden)
                    .ThenInclude(s=> s.Stemmen)
                .Include(p => p.PollGebruikers).SingleAsync(i=> i.PollID == id);

            foreach(var pollGebruiker in poll.PollGebruikers)
            {
                _context.PollGebruikers.Remove(pollGebruiker);
            }

            foreach(var antwoord in poll.Antwoorden)
            {
                foreach(var stem in antwoord.Stemmen)
                {
                    _context.Stemmen.Remove(stem);
                }

                _context.Antwoorden.Remove(antwoord);
            }

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();

            

            return poll;
        }

        // DELETE: api/PollGebruiker/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PollGebruiker>> DeletePollGebruiker(long id)
        {
            var pollGebruiker = await _context.PollGebruikers.FindAsync(id);
            if (pollGebruiker == null)
            {
                return NotFound();
            }

            _context.PollGebruikers.Remove(pollGebruiker);
            await _context.SaveChangesAsync();

            return pollGebruiker;
        }

        private bool PollGebruikerExists(long id)
        {
            return _context.PollGebruikers.Any(e => e.PollGebruikerID == id);
        }
    }
}
