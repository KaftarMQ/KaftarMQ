using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Broker;

public class BrokerController
{
    private readonly BrokerDbContext _context;

    public BrokerController(BrokerDbContext context)
    {
        _context = context;
    }

    [HttpPost("push")]
    public async Task<IActionResult> PushMessage(Guid key, string value)
    {
        var message = new Message(key, value);
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return new OkResult();
    }

    [HttpGet("pull")]
    public async Task<IActionResult> PullMessage(string key)
    {
        // Ensure that the query is built as IQueryable
        IQueryable<Message> query = _context.Messages;
        var message = await query
            .Where(m => m.Id.ToString() == key) // This now explicitly uses IQueryable's Where
            .OrderBy(m => m.Id) // Again, using IQueryable's OrderBy
            .FirstOrDefaultAsync(); // Asynchronously find the first or default message

        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return Ok(message); // Corrected: use Ok method to return an OkObjectResult with 'message'
        }

        return NotFound("No message found for the given key.");
    }



    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(string key, string clientAddress)
    {
        var subscriber = new Subscriber
        (
            Guid.NewGuid(),
            clientAddress,
            Guid.Parse(key)
        );

        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();
        return Ok($"Subscribed to key: {key}");
    }
}