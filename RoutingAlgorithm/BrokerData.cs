namespace Syncer;

public class BrokerData
{
    public BrokerData(string Url, bool IsFailed)
    {
        this.Url = Url;
        this.IsFailed = IsFailed;
    }

    public string Url { get; init; }
    public bool IsFailed { get; set; }

}