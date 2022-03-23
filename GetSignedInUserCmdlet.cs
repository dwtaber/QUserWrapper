namespace QUserWrapper;

[Cmdlet(VerbsCommon.Get,"SignedInUser")]
public class GetSignedInUser : PSCmdlet
{
    [Parameter(Mandatory = false, Position = 0)]
    public string[] ComputerName { get; set; } = {""};

    [Parameter(Mandatory = false, Position = 1)]
    public double Timeout { get; set; } = 60;

    protected override void ProcessRecord()
    {
        var pendingTasks = new List<Task<List<QUserResult>>>();
        
        foreach (var name in ComputerName)
        {
            var task = QUserFactory.GetQUserAsync(name, Timeout);
            pendingTasks.Add(task);
        }

        while (pendingTasks.Any())
        {
            var task = Task.WhenAny(pendingTasks).Result;
            var results = task.Result;
            foreach (var result in results)
            {
                WriteObject(result);
            }
            pendingTasks.Remove(task);
        }
    }
}