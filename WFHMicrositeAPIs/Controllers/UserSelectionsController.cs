using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFHMicrositeAPIs.Models;

namespace MicrositeAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserSelectionsController : ControllerBase
    {
        private readonly WFHMicrositeContext _context;

        public UserSelectionsController(WFHMicrositeContext context)
        {
            _context = context;
        }

        // GET: api/UserSelections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserSelection>>> GetUserSelection()
        {
            return await _context.UserSelection.ToListAsync();
        }

        // GET: api/UserSelections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<UserSelection>>> GetUserSelection(int id)
        {
            var userSelections = await _context.UserSelection.Where(x => x.UserId == id).ToListAsync();

            if (userSelections.Count == 0)
            {
                return NotFound();
            }

            return userSelections;
        }

        // PUT: api/UserSelections/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserSelection(int id, UserSelection userSelection)
        {
            if (id != userSelection.UserSelectionId)
            {
                return BadRequest();
            }

            _context.Entry(userSelection).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserSelectionExists(id))
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

        // POST: api/UserSelections
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UserSelection>> PostUserSelection(UserSelection userSelection)
        {
            _context.UserSelection.Add(userSelection);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserSelection", new { id = userSelection.UserSelectionId });
        }

        // DELETE: api/UserSelections/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUserSelection(int id)
        {
            var userSelections = await _context.UserSelection.Where(x => x.UserId == id).ToListAsync();
            if (userSelections.Count == 0)
            {
                return NotFound();
            }

            _context.UserSelection.RemoveRange(userSelections);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserSelectionExists(int id)
        {
            return _context.UserSelection.Any(e => e.UserSelectionId == id);
        }
    }
}
