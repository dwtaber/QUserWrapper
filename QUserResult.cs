namespace QUserWrapper;

public class QUserResult
{
    public string ComputerName { get; internal set; }
    public string Username { get; internal set; }
    public string SessionName { get; internal set; }
    public int ID { get; internal set; }
    public QUserState State { get; internal set; }
    public TimeSpan? IdleTime { get; internal set; }
    public DateTime LogOnTime { get; internal set; }
    public bool IsCurrentSession { get; internal set; }
    [Hidden]
    public string[] Substrings { get; internal set;}
    [Hidden]
    public string RawResult { get; internal set; }

    public static QUserResult Parse (string rawResult, string computerName = null)
    {
        var substrings = new List<string>( Regex.Split( rawResult.Substring(1),@"\s{2,}" ) );
        substrings.Insert( 0, rawResult.Substring(0,1) );
        
        // Disconnected sessions lack a session name, throwing off the indexing of subsequent substrings.
        if (substrings.Contains("Disc"))
        {
            substrings.Insert(2, "");
        }

        int parsedID = 0;
        bool canParseID = int.TryParse(substrings[3], out parsedID);
        TimeSpan parsedIdle = TimeSpan.FromTicks(0);
        bool idleContainsColon = substrings[5].Contains(":");
        bool canParseIdle = TimeSpan.TryParse(substrings[5], out parsedIdle);
        return new QUserResult()
        {
            // Prevent empty ComputerName when querying the local machine.
            ComputerName = string.IsNullOrWhiteSpace(computerName) ? Environment.MachineName : computerName,
            IsCurrentSession = (substrings[0] == ">") ? true : false,
            Username = substrings[1],
            SessionName = substrings[2],
            ID = parsedID,
            State = Enum.Parse<QUserState>(substrings[4]),
            IdleTime = ( (idleContainsColon == false) && (canParseIdle == true) ) ? TimeSpan.FromMinutes(double.Parse(substrings[5])) : parsedIdle,
            LogOnTime = DateTime.Parse(substrings[6]),
            RawResult = rawResult,
            Substrings = substrings.ToArray()
        };

    }
}