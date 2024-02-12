using System.Text;
using ClientLibrary;

var keys = Enumerable.Range(0, 4);

var tasks = keys.Select(key => Task.Run(async () =>
{
    int value = 0;
    while (true)
    {
        await Task.Delay(400);
        var v = value.ToString();
        Console.WriteLine($"pushed : {key.ToString()} : {v}");
        API.Push(key.ToString(), Encoding.UTF8.GetBytes(v));
        value++;
    }
})).ToList();

await Task.WhenAll(tasks);