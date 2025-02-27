using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Models;

namespace MyApp.Namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PlayersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpOptions]
        public IActionResult Preflight()
        {
            return Ok();
        }

        private static readonly Random random = new Random();

        // GET: api/players
        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _context.Players.ToListAsync();
            return Ok(players);
        }

        // GET: api/players/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            Player player = await _context.Players.FirstOrDefaultAsync(player => player.Id == id);
            if (player == null)
            {
                return NotFound("Player not found");
            }

            return Ok(player);
        }

        // POST: api/players
        [HttpPost]
        public async Task<IActionResult> AddPlayer([FromBody] Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPlayers), new { id = player.Id }, player);
        }

        // // DELETE: api/players/1
        // [HttpDelete("{id}")]
        // public IActionResult DeletePlayer(int id)
        // {
        //     Player player = Players.FirstOrDefault(m => m.Id == id);
        //     if (player == null)
        //         return NotFound("Player not found");

        //     Players.RemoveAt(id);
        //     return NoContent();
        // }

    }
}
