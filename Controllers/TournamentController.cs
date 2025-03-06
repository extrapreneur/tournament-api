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

        [HttpPut("update-winner/{matchId}")]
        public async Task<IActionResult> UpdateWinner([FromRoute] int matchId, [FromBody] UpdateWinnerRequest request)
        {
            try
            {
                await SaveUpdatedWinner(matchId, request.WinnerId);
                return Ok(new { message = "Winner updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while updating the winner." });
            }
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
            var firstRoundMatches = new List<Match>();

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
                firstRoundMatches.Add(byeMatch);
            }

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

                // Lägg till vinnarna av byeMatches till nästa runda
                foreach (var match in previousRoundMatches)
                {
                    if (match.Player2Id == null && match.WinnerId.HasValue)
                    {
                        var nextMatch = nextRoundMatches.FirstOrDefault(m => m.Player1Id == null || m.Player2Id == null);
                        if (nextMatch != null)
                        {
                            if (nextMatch.Player1Id == null)
                            {
                                nextMatch.Player1Id = match.WinnerId;
                            }
                            else if (nextMatch.Player2Id == null)
                            {
                                nextMatch.Player2Id = match.WinnerId;
                            }
                            match.NextMatch = nextMatch; // Sätt NextMatch för byeMatch
                        }
                    }
                }

                previousRoundMatches = nextRoundMatches;
            }

            _context.Matches.AddRange(matches);
            await _context.SaveChangesAsync();
        }

        [HttpGet("match/{id}")]
        public async Task<Match> GetMatch(int id)
        {
            var match = await _context.Matches
                .Include(match => match.NextMatch)
                .FirstOrDefaultAsync(match => match.Id == id);

            if (match == null)
            {
                throw new InvalidOperationException("Match not found.");
            }

            return match;
        }

        private async Task<bool> CheckIfThereAreMatches()
        {
            return await _context.Matches.AnyAsync();
        }

        private async Task SaveUpdatedWinner(int matchId, int winnerId)
        {
            var match = await GetMatch(matchId);

            if (match == null)
            {
                throw new InvalidOperationException("Match not found.");
            }

            var winner = await _context.Players.FindAsync(winnerId);
            var loser = await _context.Players.FindAsync(match.Player1Id == winnerId ? match.Player2Id : match.Player1Id);

            match.WinnerId = winnerId;
            match.Decided = true;

            await SetWinnerAsPlayerInNextMatch(match, winnerId, loser.Id);
            await RemoveLoserInTree(match, loser.Id);
            
            _context.Matches.Update(match);


            await _context.SaveChangesAsync();
        }

        private async Task SetWinnerAsPlayerInNextMatch(Match match, int winnerId, int loserId)
        {
            var nextMatch = match.NextMatch;
            if (nextMatch == null)
            {
                return;
            }

            var currentPlayers = new HashSet<int?> { winnerId, loserId };
            var nextPlayers = new HashSet<int?> { nextMatch.Player1Id, nextMatch.Player2Id };

            var commonPlayers = currentPlayers.Intersect(nextPlayers).ToList();

            if (commonPlayers.Any())
            {
                var commonPlayer = commonPlayers.First();
                if (commonPlayer != winnerId)
                {
                    if (nextMatch.Player1Id == commonPlayer)
                    {
                        nextMatch.Player1Id = winnerId;
                    }
                    else if (nextMatch.Player2Id == commonPlayer)
                    {
                        nextMatch.Player2Id = winnerId;
                    }
                }
            }
            else
            {
                if (nextMatch.Player1Id == null)
                {
                    nextMatch.Player1Id = winnerId;
                }
                else if (nextMatch.Player2Id == null)
                {
                    nextMatch.Player2Id = winnerId;
                }
            }

            _context.Matches.Update(nextMatch);
            await _context.SaveChangesAsync();
        }

        private async Task RemoveLoserInTree(Match match, int loserId)
        {
            if (match == null)
            {
                return;
            }

            var nextMatch = match.NextMatch;

            while (nextMatch != null)
            {
                if (nextMatch.Player1Id == loserId)
                {
                    nextMatch.Player1Id = null;
                    nextMatch.Decided = false;
                    nextMatch.WinnerId = null;
                }
                else if (nextMatch.Player2Id == loserId)
                {
                    nextMatch.Player2Id = null;
                    nextMatch.Decided = false;
                    nextMatch.WinnerId = null;
                }

                _context.Matches.Update(nextMatch);
                await _context.SaveChangesAsync();

                nextMatch = nextMatch.NextMatch;
                // nextMatch = await _context.Matches
                //             .Include(m => m.NextMatch)
                //             .FirstOrDefaultAsync(m => m.Id == nextMatch.NextMatch.Id);
            }
        }

    }

}
