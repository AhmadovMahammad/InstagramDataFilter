using RequestParser.CoR.Contracts;
using RequestParser.CoR.Implementations;
using RequestParser.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace RequestParser;

internal static class Program
{
    public static void Main(string[] args)
    {
        // Parse arguments
        string filePath = args.Length > 0 ? args[0] : string.Empty;
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("No file path provided.");
            return;
        }

        // Build the chain of responsibility
        IHandler handler = new ArgumentNotEmptyHandler();
        handler.SetNext(new FileExistsHandler()).SetNext(new FileExtensionHandler("json"));

        if (!handler.Handle(filePath))
        {
            Console.WriteLine("Invalid file.");
            return;
        }

        // All validations passed, Process the file
        List<RecentFollowRequest> recentFollowRequests = ProcessFile(filePath);

        // Write the output if we have data
        if (recentFollowRequests.Count != 0) WriteToFile(filePath, recentFollowRequests);
        else Console.WriteLine("No recent follow requests to write.");
    }

    private static List<RecentFollowRequest> ProcessFile(string filePath)
    {
        List<RecentFollowRequest> recentFollowRequests = new();

        try
        {
            using FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using JsonDocument jsonDocument = JsonDocument.Parse(fileStream);

            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("relationships_permanent_follow_requests", out JsonElement followRequests))
            {
                foreach (JsonElement request in followRequests.EnumerateArray())
                {
                    foreach (JsonElement stringListData in request.GetProperty("string_list_data").EnumerateArray())
                    {
                        var followRequest = new RecentFollowRequest(stringListData.GetProperty("timestamp").GetInt64())
                        {
                            Username = stringListData.GetProperty("value").GetString() ?? string.Empty,
                            Url = stringListData.GetProperty("href").GetString() ?? string.Empty,
                        };

                        recentFollowRequests.Add(followRequest);
                    }
                }
            }
        }
        catch (JsonException)
        {
            Console.WriteLine("Invalid JSON format.");
        }
        catch (Exception)
        {
            Console.WriteLine("An error occurred while parsing the JSON file.");
        }

        return recentFollowRequests;
    }

    private static void WriteToFile(string filePath, List<RecentFollowRequest> recentFollowRequests)
    {
        string outputFilePath = $"{Path.GetDirectoryName(filePath)}\\{Path.GetFileNameWithoutExtension(filePath)}_readable.txt";

        try
        {
            int maxUsernameLength = recentFollowRequests.Max(r => r.Username.Length);
            int maxTimestampLength = recentFollowRequests.Max(r => r.Date.Length);

            StringBuilder stringBuilder = new();

            // Write the header with column names, adjusted for padding
            stringBuilder.Append("№".PadRight(3)).Append('\t')
                         .Append("Username".PadRight(maxUsernameLength + 5)).Append('\t')
                         .Append("Timestamp".PadRight(maxTimestampLength + 5)).Append('\t')
                         .AppendLine("URL");

            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(stringBuilder);
                stringBuilder.Clear();

                // Write the data rows
                for (int i = 0; i < recentFollowRequests.Count; i++)
                {
                    var followRequest = recentFollowRequests[i];

                    stringBuilder.Append((i + 1).ToString().PadRight(3)).Append('\t')
                                 .Append(followRequest.Username.PadRight(maxUsernameLength + 5)).Append('\t')
                                 .Append(followRequest.Date.PadRight(maxTimestampLength + 5)).Append('\t')
                                 .AppendLine(followRequest.Url);
                }

                streamWriter.WriteLine(stringBuilder);
            }

            OpenFile(outputFilePath);
            Console.WriteLine($"Data has been successfully written to {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
        }
    }

    private static void OpenFile(string outputFilePath)
    {
        try
        {
            string powershellCommand = $"Invoke-Item '{outputFilePath}'";

            using Process process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = powershellCommand,
                UseShellExecute = false
            };

            process.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to open the file: {ex.Message}");
        }
    }
}