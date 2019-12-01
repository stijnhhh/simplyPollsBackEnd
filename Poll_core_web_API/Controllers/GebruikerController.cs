using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Poll_core_web_API.Models;
using Poll_core_web_API.Services;

namespace Poll_core_web_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GebruikerController : ControllerBase
    {
        private IUserService _userService;

        private readonly PollContext _context;

        public GebruikerController(PollContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        [Route("getcount")]
        public async Task<ActionResult<int>> GetCount()
        {
            return await _context.Gebruikers.CountAsync();
        }

        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]Gebruiker gebruikerParam)
        {
            var gebruiker = _userService.Authenticate(gebruikerParam.Email, gebruikerParam.Wachtwoord);

            if (gebruiker == null)
            {
                return BadRequest(new { message = "Gebruikersnaam of wachtwoord is niet correct" });
            } else
            {
                return Ok(gebruiker);
            }
        }

        [HttpGet]
        [Route("getGebruikerByEmail")]
        public async Task<ActionResult<Gebruiker>> GetGebruikerByEmail(string email)
        {
            try
            {
                return await _context.Gebruikers.SingleAsync(i => i.Email == email);
            } catch
            {
                return NoContent();
            }
        }

        // GET: api/Gebruiker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gebruiker>>> Getgebruikers()
        {
            return await _context.Gebruikers.ToListAsync();
        }

        // GET: api/Gebruiker/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Gebruiker>> GetGebruiker(long id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);

            if (gebruiker == null)
            {
                return NotFound();
            }

            return gebruiker;
        }

        // PUT: api/Gebruiker/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGebruiker(long id, Gebruiker gebruiker)
        {
            if (id != gebruiker.GebruikerID)
            {
                return BadRequest();
            }

            _context.Entry(gebruiker).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GebruikerExists(id))
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

        // POST: api/Gebruiker
        [HttpPost]
        public async Task<ActionResult<Gebruiker>> PostGebruiker(Gebruiker gebruiker)
        {
            try
            {
                Gebruiker gebruiker1 = await _context.Gebruikers.Where(e => e.Email == gebruiker.Email).FirstOrDefaultAsync();

                if(gebruiker1.Gebruikersnaam == null)
                {
                    gebruiker1.Gebruikersnaam = gebruiker.Gebruikersnaam;
                    gebruiker1.Wachtwoord = gebruiker.Wachtwoord;

                    _context.Entry(gebruiker1).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!GebruikerExists(gebruiker1.GebruikerID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return gebruiker1;
                } else
                {
                    return NoContent();
                }
            } catch
            {
                _context.Gebruikers.Add(gebruiker);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGebruiker", new { id = gebruiker.GebruikerID }, gebruiker);
            }
        }

        // DELETE: api/Gebruiker/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Gebruiker>> DeleteGebruiker(long id)
        {
            var gebruiker = await _context.Gebruikers.FindAsync(id);
            if (gebruiker == null)
            {
                return NotFound();
            }

            _context.Gebruikers.Remove(gebruiker);
            await _context.SaveChangesAsync();

            return gebruiker;
        }

        private bool GebruikerExists(long id)
        {
            return _context.Gebruikers.Any(e => e.GebruikerID == id);
        }
    }
}
