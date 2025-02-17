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
            var members = await _context.Members.ToListAsync();
            var randomizedMembers = members.OrderBy(m => random.Next()).ToList();
            var matches = CreateTournamentTree(randomizedMembers);
            return Ok(matches);
        }

        private List<Match> CreateTournamentTree(List<Member> members)
        {
            if (members.Count == 0)
            {
                return new List<Match>();
            }

            if (members.Count == 1)
            {
                return new List<Match> { new Match { Player1 = members[0], Player2 = null } };
            }

            var matches = new List<Match>();
            for (int i = 0; i < members.Count; i += 2)
            {
                if (i + 1 < members.Count)
                {
                    matches.Add(new Match { Player1 = members[i], Player2 = members[i + 1] });
                }
                else
                {
                    matches.Add(new Match { Player1 = members[i], Player2 = null });
                }
            }

            var nextRoundMembers = matches.SelectMany(m => new[] { m.Player1, m.Player2 }).Where(m => m != null).ToList();
            if (nextRoundMembers.Count == members.Count)
            {
                // If the next round members count is the same as the current members count, break the recursion
                return matches;
            }

            matches.AddRange(CreateTournamentTree(nextRoundMembers));
            return matches;
        }
    }
}