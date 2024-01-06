using Con4sumer;

namespace Producer;

public class MessageGenerator
{
    public static async Task<Message> generate()
    {
        Message message = new Message();
        message.MessageId = generateKey();
        message.Content = generateValue();
        return message;
    }

    private static string generateKey()
    {
        throw new NotImplementedException();
    }
    
    private static string generateValue()
    {
        throw new NotImplementedException();
    }

}