using Pathoschild.Http.Client;

namespace Producer;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Enter Mode:");
        var mode = Console.ReadLine();
        var sender = new Sender();

        if (mode == "auto")
        {
            sender.Produce(new AutoMessageGenerator()).Wait();
        }
        else
        {
            sender.Produce(new ManualMessageGenerator()).Wait();
        }
    }
}