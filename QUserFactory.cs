namespace QUserWrapper;
public class QUserFactory
{
    public static async Task<List<QUserResult>> GetQUserAsync (string computerName, TimeSpan timeout)
    {

        using (var process = new Process())
        {
            process.StartInfo = new ProcessStartInfo()
            {
                ArgumentList = {$"/SERVER:{computerName}"},
                CreateNoWindow = true,
                FileName = "QUser.exe",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            process.Start();            
            await Task.WhenAny(process.WaitForExitAsync(), Task.Delay(timeout));
            
            var parsedResults = new List<QUserResult>();
            
            // Empty list if the process didn't finish.
            if (process.HasExited == false) {return parsedResults;}
            
            // Get rid of header line.
            process.StandardOutput.ReadLine();
            while (process.StandardOutput.EndOfStream == false)
            {
                var rawResult = process.StandardOutput.ReadLine();
                var parsedResult = QUserResult.Parse(rawResult, computerName);
                parsedResults.Add(parsedResult);
            }
            return parsedResults;
        }
    }
    public static async Task<List<QUserResult>> GetQUserAsync (string computerName, double timeoutSeconds)
    {        
        var timeout = TimeSpan.FromSeconds(timeoutSeconds);
        return await GetQUserAsync (computerName, timeout);
    }

    #region Don't think any of these got used.
    /*

    public static Process Create (string computerName)
    {
        return new Process
        {
            StartInfo = new ProcessStartInfo
            {
                ArgumentList = {$"/SERVER:{computerName}"},
                CreateNoWindow = true,
                FileName = "QUser.exe",
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };
    }

    public static Task[] CreateMany (string[] computerNames)
    {
        List<Task> Tasks = new();
        foreach (var name in computerNames)
        {
            var proc = Create(name);
            var task = Task.Run( () => {
                                            proc.Start();
                                            proc.WaitForExit();
                                            return ReadAfterHeader(proc);
                                       }
                                );
            Tasks.Add(task);
        }
        return Tasks.ToArray();
    }

    public static string[] ReadAfterHeader (Process process)
    {
        var afterHeader = new List<string>();
        var output = process.StandardOutput;
        output.ReadLine();
        while (output.EndOfStream == false)
        {
            afterHeader.Add(output.ReadLine());
        }
        return afterHeader.ToArray();
    }
    */
    #endregion
}