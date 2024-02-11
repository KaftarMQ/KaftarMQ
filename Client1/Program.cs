using System.Text;
using ClientLibrary;

API.Push("Khalafi Eshgh 1", Encoding.UTF8.GetBytes("Message 1"));
API.Push("Khalafi Eshgh 1", Encoding.UTF8.GetBytes("Message 2"));
API.Push("Khalafi Eshgh 2", Encoding.UTF8.GetBytes("Message 1"));
API.Push("Khalafi Eshgh 2", Encoding.UTF8.GetBytes("Message 2"));

Action<(string key, byte[] value)> f = message =>
{
    var value = Encoding.UTF8.GetString(message.value);
    Console.WriteLine($"Received message with key: \"{message.key}\", value: \"{value}\"");
};

f(API.Pull());

API.Subscribe(f);

Thread.Sleep(TimeSpan.FromMinutes(5));