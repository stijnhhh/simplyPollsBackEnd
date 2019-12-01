using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poll_core_web_API.Models;

namespace Poll_core_web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollController : ControllerBase
    {
        private readonly PollContext _context;

        public PollController(PollContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getcount")]
        public async Task<ActionResult<int>> GetCount()
        {
            return await _context.Polls.CountAsync();
        }

        // GET: api/Poll
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poll>>> Getpolls()
        {
            return await _context.Polls.Include(a => a.Antwoorden).Include(p => p.PollGebruikers).ToListAsync();
        }

        // GET: api/Poll/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Poll>> GetPoll(long id)
        {
            var poll = await _context.Polls.FindAsync(id);

            if (poll == null)
            {
                return NotFound();
            }

            return poll;
        }

        // PUT: api/Poll/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoll(long id, Poll poll)
        {
            if (id != poll.PollID)
            {
                return BadRequest();
            }

            _context.Entry(poll).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PollExists(id))
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

        // POST: api/Poll
        [HttpPost]
        public async Task<ActionResult<Poll>> PostPoll(Poll poll)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            _context.Polls.Add(poll);
            await _context.SaveChangesAsync();

            PollGebruiker pollGebruiker = new PollGebruiker();
            pollGebruiker.Admin = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID);
            pollGebruiker.Actief = true;
            pollGebruiker.Gebruiker = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID);
            pollGebruiker.Poll = await _context.Polls.LastAsync();
            pollGebruiker.PollGebruikerID = 0;

            _context.PollGebruikers.Add(pollGebruiker);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPoll", new { id = poll.PollID }, poll);
        }

        // DELETE: api/Poll/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Poll>> DeletePoll(long id)
        {
            var poll = await _context.Polls.FindAsync(id);
            if (poll == null)
            {
                return NotFound();
            }

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();

            return poll;
        }

        private bool PollExists(long id)
        {
            return _context.Polls.Any(e => e.PollID == id);
        }
    }
}
