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
    public TaskStatus Status { get; set; }
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
            Debug.WriteLine($"Data file {FilePath} exists.");
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
        try
        {
            var newId = tasks.Max(t => t.Id) + 1;
            return newId;
        }
        catch (InvalidOperationException)
        {
            return 1;
        }
    }

    private static void WriteTasksToJsonFile(List<Task> tasks)
    {
        var jsonString = JsonSerializer.Serialize(tasks, JsonSerializeOptions);
        if (FilePath != null) File.WriteAllText(FilePath, jsonString);
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
            Status = TaskStatus.ToDo
        };

        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");
        var tasks = ReadData();
        tasks.Add(task);
        
        WriteTasksToJsonFile(tasks);
        Console.WriteLine(
            $"Task added to list. Task id: {task.Id}, Task status: {task.Status}, Task Description: {task.Description}");
    }

    public static void UpdateTask(Command command)
    {
        Debug.Assert(command.Type == CommandType.Update, "Command type is not `update`");
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");
        
        var tasks = ReadData();
        var index = tasks.FindIndex(t => t.Id == command.Id);
        if (index != -1)
        {
            var task = tasks[index];
            task.Description = command.Description;
            task.UpdatedAt = DateTime.Now;
            tasks[index] = task;
        
            WriteTasksToJsonFile(tasks);           
            Console.WriteLine(
                $"Task updated. Task id: {task.Id}, Task status: {task.Status}, Task Description: {task.Description}");
        }
        else
        {
            Console.WriteLine($"Task with id {command.Id} not found!");
        }
    }

    public static void DeleteTask(Command command)
    {
        Debug.Assert(command.Type == CommandType.Delete, "Command type is not `delete`") ;
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");

        var tasks = ReadData();
        tasks.RemoveAll(t => command.IdList.Contains(t.Id));
        WriteTasksToJsonFile(tasks);
    }

    public static void ListTask(Command command)
    {
        Debug.Assert(command.Type == CommandType.List, "Command type is not `list`");
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");

        var tasks = ReadData();
        tasks = tasks.FindAll(t => command.StatusList.Contains(t.Status));
        foreach (var task in tasks)
        {
            Console.WriteLine("-----------------------------");    
            Console.WriteLine($"Task id: {task.Id}");
            Console.WriteLine($"Task description: {task.Description}");
            Console.WriteLine($"Task status: {task.Status}");
            Console.WriteLine($"Created at {task.CreatedAt}");
            Console.WriteLine($"Updated at {task.UpdatedAt}");
            Console.WriteLine("-----------------------------");    
        }
    }

    public static void MarkToDo(Command command)
    {
        Debug.Assert(command.Type == CommandType.MarkToDo, "Command type is not `mark-todo`");
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");

        var tasks = ReadData();
        for (int i = 0; i < tasks.Count; i++)
        {
            if (command.IdList.Contains(tasks[i].Id))
            {
                var tempTask = tasks[i];
                tempTask.Status = TaskStatus.ToDo;
                tempTask.UpdatedAt = DateTime.Now;
                tasks[i] = tempTask;
            }
        }
        
        WriteTasksToJsonFile(tasks);
    }

    public static void MarkInProgress(Command command)
    {
        Debug.Assert(command.Type == CommandType.MarkInProgress, "Command type is not `mark-in-progress`");
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");

        var tasks = ReadData();
        for (int i = 0; i < tasks.Count; i++)
        {
            if (command.IdList.Contains(tasks[i].Id))
            {
                var tempTask = tasks[i];
                tempTask.Status = TaskStatus.InProgress;
                tempTask.UpdatedAt = DateTime.Now;
                tasks[i] = tempTask;
            }
        }
        
        WriteTasksToJsonFile(tasks);
    }
    
    public static void MarkDone(Command command)
    {
        Debug.Assert(command.Type == CommandType.MarkDone, "Command type is not `mark-done`");
        Debug.Assert(FilePath != null && File.Exists(FilePath), "Data file path does not exist");

        var tasks = ReadData();
        for (int i = 0; i < tasks.Count; i++)
        {
            if (command.IdList.Contains(tasks[i].Id))
            {
                var tempTask = tasks[i];
                tempTask.Status = TaskStatus.Done;
                tempTask.UpdatedAt = DateTime.Now;
                tasks[i] = tempTask;
            }
        }
        
        WriteTasksToJsonFile(tasks);
    }

    public static void PrintHelp(Command command)
    {
        Debug.Assert(command.Type == CommandType.Help, "Command type is not `help`");
        Console.WriteLine("""

                          -----------------------
                          Task Tracker CLI
                          -----------------------
                          Usage:
                            add <description>                     Add a new task with a description
                            update <id> <description>             Update the description of an existing task
                            delete <id1> [id2] [id3] ...          Delete one or more tasks by ID
                            mark-todo <id1> [id2] [id3] ...       Mark one or more tasks as 'todo'
                            mark-in-progress <id1> [id2] [id3] ... Mark one or more tasks as 'in-progress'
                            mark-done <id1> [id2] [id3] ...       Mark one or more tasks as 'done'
                            list [status]                         List tasks; optionally filter by status:
                                                                  - 'todo', 'in-progress', or 'done'
                                                                  - If no status is provided, lists all tasks
                          
                          Examples:
                            add "Buy groceries"
                            update 3 "Buy milk and bread"
                            delete 1 4 5
                            mark-done 2 7
                            list todo
                            list

                          """);
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