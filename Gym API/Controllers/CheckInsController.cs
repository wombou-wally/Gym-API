using Gym_API.Data;
using Gym_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gym_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInsController : ControllerBase
    {
        private readonly GymContext  _context;

        public CheckInsController(GymContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckIn>>> GetCheckIns()
        {
            var checks = await _context.CheckIns.OrderBy(c => c.Id).ToListAsync();
            return Ok(checks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetCheckIn(int id)
        {
            var check = await _context.CheckIns.FindAsync(id);
            if (check == null)
            {
                return NotFound();
            }
            return Ok(check);
        }
        [HttpPost]
        public async Task<ActionResult<CheckIn>> PostCheckIn(CheckIn checkIn)
        {
            bool memberExists = await _context.Members.AnyAsync(m => m.Id == checkIn.MemberId);
            if (!memberExists)
            {
                return BadRequest("Member does not exist.");
            }

            checkIn.member = null;
            checkIn.CheckInTime = DateTime.UtcNow;

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.Id }, checkIn);
        }
    }
}
