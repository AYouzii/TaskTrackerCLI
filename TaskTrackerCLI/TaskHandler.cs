// using System.Reflection.Metadata;

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskTrackerCLI;

public enum TaskStatus
{
    ToDo,
    InProgress,
    Done
}

public struct Task
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public TaskStatus TaskStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public static class TaskHandler
{
    private static readonly string? FilePath;
    private static readonly JsonSerializerOptions JsonSerializeOptions;

    static TaskHandler()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        FilePath = Path.Combine(baseDir, "data.json");
        if (File.Exists(FilePath))
        {
            Console.WriteLine($"Data file {FilePath} exists.");
        }
        else
        {
            File.Create(FilePath).Close();
            File.WriteAllText(FilePath, "[]", Encoding.UTF8);
            Console.WriteLine($"Data file {FilePath} created.");
        }

        JsonSerializeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    private static int GetNewId()
    {
        var tasks = ReadData();
        tasks.Sort((x, y) => x.Id.CompareTo(y.Id));
        var newId = tasks.Last().Id + 1;
        return newId;
    }
    
    public static void AddTask(Command command)
    {
        Debug.Assert(command.Type == CommandType.Add, "Command type is not `add`");
        var task = new Task
        {
            Description = command.Description,
            Id = GetNewId(),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            TaskStatus = TaskStatus.ToDo
        };

        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");
        var tasks = ReadData();
        tasks.Add(task);
        
        var jsonString = JsonSerializer.Serialize(tasks, JsonSerializeOptions);

        File.WriteAllText(FilePath, jsonString);
        Console.WriteLine($"task id: {task.Id}, task status: {task.TaskStatus}");
    }

    private static List<Task> ReadData()
    {
        if (FilePath == null)
        {
            Console.WriteLine("Data file path is null, return empty task list");
            return [];
        }
        var jsonString = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(jsonString))
        {
            Console.WriteLine("Json string is empty");
            return [];
        }

        try
        {
            var tasks = JsonSerializer.Deserialize<List<Task>>(jsonString, JsonSerializeOptions);
            return tasks ?? [];
        }
        catch (JsonException err)
        {
            Console.WriteLine(err);
            Console.WriteLine("Json syntax error, return empty task list");
            return [];
        }
        
    }
}