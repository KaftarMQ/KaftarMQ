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
    public async Task<IActionResult> PushMessage(string key, string value, Guid id)
    {
        var message = new Message(key, value, id);
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return new OkResult();
    }

    [HttpGet("pull")]
    public async Task<IActionResult> PullMessage(string key)
    {
        IQueryable<Message> query = _context.Messages;
        var message = await query
            .Where(m => m.Id.ToString() == key)
            .OrderBy(m => m.Id)
            .FirstOrDefaultAsync();

        if (message != null)
        {
            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return new OkObjectResult(message);
        }

        return new NotFoundResult();
    }


    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe(string key, string clientAddress)
    {
        var subscriber = new Subscriber
        (
            clientAddress
        );

        _context.Subscribers.Add(subscriber);
        await _context.SaveChangesAsync();
        return new OkObjectResult($"Subscribed to key: {key}");
    }
}