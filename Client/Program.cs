using System.Text;
using Client;

API.Push("Khalafi Eshgh 1", "Message 1"u8.ToArray());
API.Push("Khalafi Eshgh 1", "Message 2"u8.ToArray());
API.Push("Khalafi Eshgh 2", "Message 1"u8.ToArray());
API.Push("Khalafi Eshgh 2", "Message 2"u8.ToArray());

API.Subscribe(message =>
{
    var value = Encoding.UTF8.GetString(message.value);
    Console.WriteLine($"Received message with key: \"{message.key}\", value: \"{value}\"");
});

Thread.Sleep(TimeSpan.FromMinutes(5));