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

            var randomizedPlayerIds = players.Select(player => player.Id).OrderBy(m => random.Next()).ToList();
            var matches = CreateTournamentTree(randomizedPlayerIds);
            await SaveMatchesToDatabase(matches);
            return Ok(matches);
        }

        private List<Match> CreateTournamentTree(List<int> playerIds)
        {
            if (playerIds.Count == 0 || playerIds == null)
            {
                return new List<Match>();
            }

            if (playerIds.Count == 1)
            {
                return new List<Match> {
                    new Match {
                        PlayerId1 = playerIds[0],
                        PlayerId2 = null,
                    }
                };
            }

            var matches = new List<Match>();

            for (int i = 0; i < playerIds.Count; i += 2)
            {
                if (i + 1 < playerIds.Count)
                {
                    matches.Add(
                        new Match
                        {
                            PlayerId1 = playerIds[i],
                            PlayerId2 = playerIds[i + 1],
                        });
                }
                else
                {
                    matches.Add(
                        new Match
                        {
                            PlayerId1 = playerIds[i],
                            PlayerId2 = null,
                        });
                }
            }

            var nextRoundPlayers = matches
                .SelectMany(m => new[] { m.PlayerId1, m.PlayerId2 })
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            if (nextRoundPlayers.Count == playerIds.Count)
            {
                // If the next round players count is the same as the current players count, break the recursion
                return matches;
            }

            if (nextRoundPlayers.Count == 0)
            {
                nextRoundPlayers = new List<int>();
            }

            matches.AddRange(CreateTournamentTree(nextRoundPlayers));
            return matches;
        }

        private async Task SaveMatchesToDatabase(List<Match> matches)
        {
            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();
        }

        public async Task<IActionResult> GetMatchesInCurrentRound([FromBody] Player player)
        {
            var matches = await _context.Matches.Where(m => m.PlayerId1 == player.Id || m.PlayerId2 == player.Id).ToListAsync();
            return Ok(matches);
        }
    }
}