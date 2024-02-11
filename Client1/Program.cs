using System.Text;
using ClientLibrary;


void f(string key, string value, int consumerId)
{
    Console.WriteLine($"{consumerId}: {key} {value}");
}

var consumerIds = Enumerable.Range(0, 1);

var tasks = consumerIds.Select(async consumerId => await Task.Run(() =>
{
    API.Subscribe((message =>
    {
        f(message.key, Encoding.UTF8.GetString(message.value), consumerId);
    }));
    
})).ToList();

await Task.Delay(100000);