﻿using Pathoschild.Http.Client;
using RoutingAlgorithm;

public class Sender
{
    internal async Task Produce(IMessageGenerator messageGenerator)
    {
        Console.WriteLine("Enter a key to start producing messages:");
        var key = Console.ReadLine();
        if (string.IsNullOrEmpty(key))
        {
            key = "default";
        }

        while (true)
        {
            var message = messageGenerator.GetNext();

            await new FluentClient(ENVIRONMENT.NGINX)
                .PostAsync("message/push")
                .WithArgument("key", key)
                .WithArgument("value", message);
        }
    }
}

class AutoMessageGenerator : IMessageGenerator
{
    public string? GetNext()
    {
        return Guid.NewGuid().ToString();
    }
}


class ManualMessageGenerator : IMessageGenerator
{
    public string? GetNext()
    {
        return Console.ReadLine();
    }
}

internal class Message
{
    public string Key { get; set; }
    public string Value { get; set; }
}