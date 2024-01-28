namespace Broker;

public record Subscriber(Guid Id, string ClientAddress, Guid PartitionKey);
