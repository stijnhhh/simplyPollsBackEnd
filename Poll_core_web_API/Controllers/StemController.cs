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
    public class StemController : ControllerBase
    {
        private readonly PollContext _context;

        public StemController(PollContext context)
        {
            _context = context;
        }

        // GET: api/Stem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stem>>> Getstemmen()
        {
            return await _context.Stemmen.ToListAsync();
        }

        // GET: api/Stem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Stem>> GetStem(long id)
        {
            var stem = await _context.Stemmen.FindAsync(id);

            if (stem == null)
            {
                return NotFound();
            }

            return stem;
        }

        [Authorize]
        [HttpGet]
        [Route("getstemmen")]
        public async Task<ActionResult<IEnumerable<Stem>>> GetStemmen()
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            var gebruiker = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID);

            var stemmen = await _context.Stemmen.Where(g => g.Gebruiker == gebruiker).Include(a => a.Antwoord).ToListAsync();

            if (stemmen == null)
            {
                return NotFound();
            }

            return stemmen;
        }

        // PUT: api/Stem/5
        [Authorize]
        [HttpPost]
        [Route("voegstemtoe")]
        public async Task<IActionResult> PostOfDeleteStem(Antwoord antwoord)
        {
            var gebruikerID = long.Parse(User.Claims.FirstOrDefault(c => c.Type == "GebruikersID").Value);

            try
            {
                Stem stem = await _context.Stemmen.Where(a => a.Antwoord == antwoord && a.Gebruiker.GebruikerID == gebruikerID).SingleAsync();

                _context.Stemmen.Remove(stem);
                await _context.SaveChangesAsync();

                return NoContent();
            } catch
            {
                var stemInsert = new Stem
                {
                    Gebruiker = await _context.Gebruikers.SingleAsync(g => g.GebruikerID == gebruikerID),
                    Antwoord = antwoord
                };

                _context.Entry(stemInsert.Gebruiker).State = EntityState.Modified;
                _context.Entry(stemInsert.Antwoord).State = EntityState.Modified;

                _context.Stemmen.Add(stemInsert);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetStem", new { id = stemInsert.StemID }, stemInsert);
            }
        }

        // POST: api/Stem
        [HttpPost]
        public async Task<ActionResult<Stem>> PostStem(Stem stem)
        {
            _context.Stemmen.Add(stem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStem", new { id = stem.StemID }, stem);
        }

        // DELETE: api/Stem/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Stem>> DeleteStem(long id)
        {
            var stem = await _context.Stemmen.FindAsync(id);
            if (stem == null)
            {
                return NotFound();
            }

            _context.Stemmen.Remove(stem);
            await _context.SaveChangesAsync();

            return stem;
        }

        private bool StemExists(long id)
        {
            return _context.Stemmen.Any(e => e.StemID == id);
        }
    }
}
