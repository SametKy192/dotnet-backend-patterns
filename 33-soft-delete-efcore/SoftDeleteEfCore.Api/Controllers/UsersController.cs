using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftDeleteEfCore.Api.Data;
using SoftDeleteEfCore.Api.Entities;

namespace SoftDeleteEfCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/users
    // Returns active users only (Global query filter !IsDeleted is applied automatically)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/users/with-deleted
    // Returns all users including soft-deleted ones (Bypasses the global query filter using IgnoreQueryFilters)
    [HttpGet("with-deleted")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        return await _context.Users
            .IgnoreQueryFilters()
            .ToListAsync();
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        // Finds active user. If user is soft-deleted, Find/First will return null/404 due to HasQueryFilter
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound("User not found or has been deleted.");
        }
        return user;
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // DELETE: api/users/{id}
    // Deletes (Soft Deletes) a user. The SoftDeleteInterceptor will catch this, cancel the deletion,
    // and turn it into a soft delete update instead.
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound("User not found or already deleted.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // POST: api/users/{id}/restore
    // Restores a soft-deleted user. Bypasses the filter to find the user, and sets IsDeleted = false.
    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreUser(int id)
    {
        var user = await _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!user.IsDeleted)
        {
            return BadRequest("User is not deleted.");
        }

        user.IsDeleted = false;
        user.DeletedAtUtc = null;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}

public record CreateUserDto(string Name, string Email);
