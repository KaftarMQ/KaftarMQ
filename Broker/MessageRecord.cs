namespace Broker;

public record MessageRecord(Guid Id, DateTime TimeStamp, Message Message);