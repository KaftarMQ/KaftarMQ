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

    public sealed class UrlRelationalComparer : IComparer<BrokerData>
    {
        public int Compare(BrokerData x, BrokerData y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return string.Compare(x.Url, y.Url, StringComparison.Ordinal);
        }
    }

    public static IComparer<BrokerData> UrlComparer { get; } = new UrlRelationalComparer();
}