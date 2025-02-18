using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp.Namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private static readonly Random random = new Random();

        public TournamentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpOptions]
        public IActionResult Preflight()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTournamentOrder()
        {
            var players = await _context.Players.ToListAsync();

            var randomizedPlayers = players.OrderBy(m => random.Next()).ToList();
            var matches = CreateTournamentTree(randomizedPlayers);
            return Ok(matches);
        }

        private List<Match> CreateTournamentTree(List<Player> players)
        {
            if (players.Count == 0)
            {
                return new List<Match>();
            }

            if (players.Count == 1)
            {
                return new List<Match> { new Match { Player1 = players[0], Player2 = null } };
            }

            var matches = new List<Match>();
            for (int i = 0; i < players.Count; i += 2)
            {
                if (i + 1 < players.Count)
                {
                    matches.Add(new Match { Player1 = players[i], Player2 = players[i + 1] });
                }
                else
                {
                    matches.Add(new Match { Player1 = players[i], Player2 = null });
                }
            }

            var nextRoundPlayers = matches.SelectMany(m => new[] { m.Player1, m.Player2 }).Where(m => m != null).ToList();
            if (nextRoundPlayers.Count == players.Count)
            {
                // If the next round players count is the same as the current players count, break the recursion
                return matches;
            }

            matches.AddRange(CreateTournamentTree(nextRoundPlayers));
            return matches;
        }

        public async Task<IActionResult> GetMatchesInCurrentRound([FromBody] Player player)
        {
            var matches = await _context.Matches.Where(m => m.Player1 == player || m.Player2 == player).ToListAsync();
            return Ok(matches);
        }
    }



}