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
    public class MembersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpOptions]
        public IActionResult Preflight()
        {
            return Ok();
        }

        private static readonly Random random = new Random();

        // GET: api/members
        [HttpGet]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _context.Members.ToListAsync();
            return Ok(members);
        }

        // // GET: api/members/1
        // [HttpGet("{id}")]
        // public IActionResult GetMemberById(int id)
        // {
        //     Member member = Members.FirstOrDefault(m => m.Id == id);
        //     if (member == null)
        //         return NotFound("Member not found");

        //     return Ok(Members[id]);
        // }

        // POST: api/members
        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMembers), new { id = member.Id }, member);
        }

        // // DELETE: api/members/1
        // [HttpDelete("{id}")]
        // public IActionResult DeleteMember(int id)
        // {
        //     Member member = Members.FirstOrDefault(m => m.Id == id);
        //     if (member == null)
        //         return NotFound("Member not found");

        //     Members.RemoveAt(id);
        //     return NoContent();
        // }

    }
}
