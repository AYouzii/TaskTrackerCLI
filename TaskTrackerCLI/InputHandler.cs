namespace TaskTrackerCLI;

public enum CommandType
{
    Add,
    Update,
    Delete,
    List,
    Help
}

public struct Command
{
    public CommandType Type { get; set; }
    public string? Description { get; set; }
    public int? Id { get; set; }
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
            Id = id,
            IsValid = true
        };
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