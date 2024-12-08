namespace RequestParser.Models;

public record RecentFollowRequest
{
    public RecentFollowRequest(long timestamp)
    {
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        Date = dateTime.AddSeconds(timestamp).ToShortDateString();
    }

    public string Username { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Date { get; } = string.Empty;
}