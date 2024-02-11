using System.Text;
using ClientLibrary;

var keys = Enumerable.Range(0, 3);

var tasks = keys.Select(key => Task.Run(() =>
{
    int value = 0;
    while (true)
    {
        Task.Delay(1000);
        var v = value.ToString();
        Console.WriteLine($"pushed : {key.ToString()} : {v}");
        API.Push(key.ToString(), Encoding.UTF8.GetBytes(v));
        value++;
    }
})).ToList();

await Task.WhenAll(tasks);