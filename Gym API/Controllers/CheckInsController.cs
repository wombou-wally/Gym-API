using Gym_API.Data;
using Gym_API.DTOs;
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
        public async Task<ActionResult<IEnumerable<CheckInResponse>>> GetCheckIns()
        {
            var checks = await _context.CheckIns
                .OrderBy(c => c.Id)
                .Select(c => new CheckInResponse
                {
                    Id = c.Id,
                    MemberId = c.MemberId,
                    CheckInTime = c.CheckInTime,
                }).ToListAsync();
            return Ok(checks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckInResponse>> GetCheckIn(int id)
        {
            var check = await _context.CheckIns
                .Where(c => c.Id == id)
                .Select(c => new CheckInResponse
                {
                    Id = c.Id,
                    MemberId = c.MemberId,
                    CheckInTime = c.CheckInTime,
                }
                ).FirstOrDefaultAsync();
            if (check == null)
            {
                return NotFound();
            }
            return Ok(check);
        }
        [HttpPost]
        public async Task<ActionResult<CheckInResponse>> PostCheckIn(CreateCheckInRequest request)
        { 
            bool memberExists = await _context.Members.AnyAsync(m => m.Id == request.MemberId);
            if (!memberExists)
            {
                return BadRequest("Member does not exist.");
            }
            var checkIn = new CheckIn
            {

                MemberId = request.MemberId,
                CheckInTime = DateTime.UtcNow
            };

            
            

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();
            var response = new CheckInResponse
            {
                Id = checkIn.Id,
                MemberId = checkIn.MemberId,
                CheckInTime = checkIn.CheckInTime
            };
            return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.Id }, response);
        }
    }
}
