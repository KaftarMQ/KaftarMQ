using System.Text;
using Pathoschild.Http.Client;

namespace Client;

public static class API
{
    public static string NGINX = "http://localhost:5000";
    
    public static void Push(string key, byte[] value)
    {
        var decodedValue = Encoding.UTF8.GetString(value);
        Console.WriteLine($"Pushing message with key: {key}, value: {decodedValue}");

        new FluentClient(NGINX)
            .PostAsync("router/Message/push")
            .GetAwaiter().GetResult();
    }

    public static (string key, byte[] value) Pull()
    {
        var message = new FluentClient(NGINX)
            .GetAsync("router/Message/pull")
            .As<Message>()
            .GetAwaiter().GetResult();

        return (message.Key, Encoding.UTF8.GetBytes(message.Value));
    }

    public static void Subscribe(Action<(string key, byte[] value)> f)
    {
        Task.Run(() =>
        {
            while (true)
            {
                f(Pull());
            }
        });
    }
}