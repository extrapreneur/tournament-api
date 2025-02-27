using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MyBackend.Data;
using MyBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyApp.Namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class TournamentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TournamentController> _logger;
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

        [HttpPost("set-tournament-order")]
        public async Task<IActionResult> SetTournamentOrder()
        {
            if (await CheckIfThereAreMatches())
            {
                return BadRequest("Tournament order already set.");
            }
            try
            {
                var players = await _context.Players.ToListAsync();
                if (players == null || players.Count == 0)
                {
                    return BadRequest("No players found.");
                }

                var randomizedPlayerIds = players.Select(player => player.Id).OrderBy(m => random.Next()).ToList();

                await CreateTournamentTree(randomizedPlayerIds);

                return Ok("Tournament order set successfully.");
            }
            catch (Exception ex)
            {
                // Logga undantaget
                Console.WriteLine($"Error in SetTournamentOrder: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                // Returnera ett internt serverfel
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while setting the tournament order.");
            }
        }

        [HttpGet("tournament-order")]
        public async Task<IActionResult> GetTournamentOrder()
        {
            var matches = await _context.Matches.ToListAsync();
            return Ok(matches.OrderBy(m => m.Round).ToList());
        }

        [HttpGet("current-round/{round}")]
        public async Task<IActionResult> GetMatchesInCurrentRound([FromRoute] int round)
        {
            var matches = await _context.Matches.Where(m => m.Round == round).ToListAsync();
            return Ok(matches);
        }

        private async Task CreateTournamentTree(List<int> playerIds)
        {
            if (playerIds == null || playerIds.Count == 0)
            {
                return;
            }

            // Kontrollera att alla playerIds finns i Players-tabellen
            var validPlayerIds = await _context.Players
                .Where(p => playerIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            if (validPlayerIds.Count != playerIds.Count)
            {
                throw new InvalidOperationException("One or more player IDs are invalid.");
            }

            var matches = new List<Match>();
            var rounds = (int)Math.Ceiling(Math.Log2(playerIds.Count));

            while (playerIds.Count % 2 != 0)  // Om ojämnt antal, ge en bye
            {
                var byePlayer = playerIds[0];
                playerIds.RemoveAt(0);

                var byeMatch = new Match
                {
                    Player1Id = byePlayer,
                    Player2Id = null,
                    WinnerId = byePlayer,
                    Round = 1,
                    Decided = true
                };

                matches.Add(byeMatch);
            }

            var firstRoundMatches = new List<Match>();

            for (int i = 0; i < playerIds.Count; i += 2)
            {
                firstRoundMatches.Add(new Match
                {
                    Player1Id = playerIds[i],
                    Player2Id = playerIds[i + 1],
                    Round = 1
                });
            }

            matches.AddRange(firstRoundMatches);

            var previousRoundMatches = firstRoundMatches;

            for (int round = 2; round <= rounds; round++)
            {
                var nextRoundMatches = new List<Match>();
                for (int i = 0; i < previousRoundMatches.Count; i += 2)
                {
                    var nextMatch = new Match { Round = round };
                    matches.Add(nextMatch);
                    nextRoundMatches.Add(nextMatch);

                    previousRoundMatches[i].NextMatch = nextMatch;
                    if (i + 1 < previousRoundMatches.Count)
                        previousRoundMatches[i + 1].NextMatch = nextMatch;
                }
                previousRoundMatches = nextRoundMatches;
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();
        }

        private async Task<Match> GetMatch(int id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return null;
            }

            return match;
        }

        private async Task<bool> CheckIfThereAreMatches()
        {
            return await _context.Matches.AnyAsync();
        }
    }
}