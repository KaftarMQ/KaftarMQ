using System.Text;
using Pathoschild.Http.Client;

namespace ClientLibrary;

public static class API
{
    public static string NGINX = "http://localhost:5274";
    
    public static void Push(string key, byte[] value)
    {
        var decodedValue = Encoding.UTF8.GetString(value);
        Console.WriteLine($"Pushing message with key: \"{key}\", value: \"{decodedValue}\"");

        new FluentClient(NGINX)
            .PostAsync("Message/push")
            .WithArgument("key", key)
            .WithArgument("value", decodedValue)
            .GetAwaiter().GetResult();
    }
    
    public static (string key, byte[] value) Pull()
    {
        while (true)
        {
            var message = InternalPull();
            if (message.HasValue)
            {
                var m = message.Value;
                return (m.key, m.value);
            }
            Thread.Sleep(100);
        }
    }

    public static void Subscribe(Action<(string key, byte[] value)> f)
    {
        Task.Run(() =>
        {
            while (true)
            {
                var message = InternalPull();
                if (message.HasValue)
                {
                    var m = message.Value;
                    f(m);
                }
                Thread.Sleep(100);
            }
        });
    }
    
    private static (string key, byte[] value)? InternalPull()
    {
        var message = new FluentClient(NGINX)
            .GetAsync("Message/pull")
            .As<Message?>()
            .GetAwaiter().GetResult();

        if (message == null)
        {
            return null;
        }
        
        return (message.Key, Encoding.UTF8.GetBytes(message.Value));
    }
}