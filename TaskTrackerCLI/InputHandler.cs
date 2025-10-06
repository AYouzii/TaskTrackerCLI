namespace TaskTrackerCLI;

public enum CommandType
{
    Add,
    Update,
    Delete,
    List,
    MarkToDo,
    MarkInProgress,
    MarkDone,
    Help
}

public struct Command
{
    public CommandType Type { get; set; }
    public string? Description { get; set; }
    public int? Id { get; set; }
    public List<int> IdList { get; set; }
    public List<TaskStatus> StatusList { get; set; }
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
}

public static class InputHandler
{
    public static Command Parse(string[] input)
    {
        var command = new Command();
        if (input.Length == 0)
        {
            command.IsValid = false;
            command.ErrorMessage = "Empty input";
            return command;
        }

        var commandParts = input;
        var commandAction = commandParts[0].ToLower();
        switch (commandAction)
        {
            case "add": return ParseAddCommand(commandParts);
            case "update": return ParseUpdateCommand(commandParts);
            case "delete": return ParseDeleteCommand(commandParts);
            case "list": return ParseListCommand(commandParts);
            case "mark-todo": return ParseMarkToDoCommand(commandParts);
            case "mark-in-progress": return ParseMarkInProgressCommand(commandParts);
            case "mark-done": return ParseMarkDoneCommand(commandParts);
            case "help": return new Command { Type = CommandType.Help, IsValid = true };
            default: return InvalidCommand($"Unknown command: {commandAction}");
        }    
    }

    private static Command ParseAddCommand(string[] commandParts)
    {
        if (commandParts.Length < 2)
        {
            return InvalidCommand("Missing task description");
        }

        var commandDescription = string.Join(" ", commandParts.Skip(1));
        return new Command
        {
            Type = CommandType.Add,
            Description = commandDescription,
            IsValid = true
        };
    }

    private static Command ParseUpdateCommand(string[] commandParts)
    {
        if (commandParts.Length != 3)
        {
            return InvalidCommand("Invalid syntax! update <id> <description>");
        }

        if (!int.TryParse(commandParts[1], out var id) || id <= 0)
        {
            return InvalidCommand("Id must be a positive integer");
        }

        return new Command
        {
            Type = CommandType.Update,
            Description = commandParts[2],
            Id = id,
            IsValid = true
        };
    }

    private static Command ParseDeleteCommand(string[] commandParts)
    {
        if (commandParts.Length < 2)
        {
            return InvalidCommand("Missing task id! Usage: delete <id1> <id2> ...");
        }

        var numberList = commandParts
            .Skip(1)
            .Select(s => (Success: int.TryParse(s, out var n), Value: n))
            .Where(result => result.Success)
            .Select(result => result.Value)
            .ToList();
        return new Command
        {
            Type = CommandType.Delete,
            IdList = numberList,
            IsValid = true
        };
    }

    private static Command ParseListCommand(string[] commandParts)
    {
        if (commandParts.Length == 1)
        {
            var statusList = new List<TaskStatus> {TaskStatus.ToDo, TaskStatus.InProgress, TaskStatus.Done};
            return new Command
            {
                Type = CommandType.List,
                StatusList = statusList,
                IsValid = true
            };
        }
        else
        {
            var statusList = commandParts
                .Skip(1)
                .Select(s => (Success: Enum.TryParse<TaskStatus>(s.Replace("-", ""), true, out var status), Value: status))
                .Where(result => result.Success)
                .Select(result => result.Value)
                .ToList();           
            return new Command
            {
                Type = CommandType.List,
                StatusList = statusList,
                IsValid = true
            };
        }
    }

    private static Command ParseMarkToDoCommand(string[] commandParts)
    {
        if (commandParts.Length < 2)
        {
            return InvalidCommand("Missing task id. Usage: mark-todo <id1> <id2> ... ");
        }
        else
        {
            var numberList = commandParts
                .Skip(1)
                .Select(s => (Success: int.TryParse(s, out var n), Value: n))
                .Where(result => result.Success)
                .Select(result => result.Value)
                .ToList();
            return new Command
            {
                Type = CommandType.MarkToDo,
                IdList = numberList,
                IsValid = true
            }; 
        }
    }
    
    private static Command ParseMarkInProgressCommand(string[] commandParts)
    {
        if (commandParts.Length < 2)
        {
            return InvalidCommand("Missing task id. Usage: mark-in-progress <id1> <id2> ... ");
        }
        else
        {
            var numberList = commandParts
                .Skip(1)
                .Select(s => (Success: int.TryParse(s, out var n), Value: n))
                .Where(result => result.Success)
                .Select(result => result.Value)
                .ToList();
            return new Command
            {
                Type = CommandType.MarkInProgress,
                IdList = numberList,
                IsValid = true
            }; 
        }
    }

    private static Command ParseMarkDoneCommand(string[] commandParts)
    {
        if (commandParts.Length < 2)
        {
            return InvalidCommand("Missing task id. Usage: mark-done <id1> <id2> ... ");
        }
        else
        {
            var numberList = commandParts
                .Skip(1)
                .Select(s => (Success: int.TryParse(s, out var n), Value: n))
                .Where(result => result.Success)
                .Select(result => result.Value)
                .ToList();
            return new Command
            {
                Type = CommandType.MarkDone,
                IdList = numberList,
                IsValid = true
            }; 
        }
    }

    private static Command InvalidCommand(string errorMessage)
    {
        return new Command
        {
            IsValid = false,
            ErrorMessage = errorMessage
        };
    }
}