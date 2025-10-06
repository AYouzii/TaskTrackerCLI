// using System;
// using System.IO;

namespace TaskTrackerCLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var command = InputHandler.Parse(args);
            if (command.IsValid)
            {
                switch (command.Type)
                {
                    case CommandType.Add: TaskHandler.AddTask(command); break;
                    case CommandType.Update: TaskHandler.UpdateTask(command); break;
                    case CommandType.Delete: TaskHandler.DeleteTask(command); break;
                    case CommandType.List: TaskHandler.ListTask(command); break;
                    case CommandType.MarkToDo: TaskHandler.MarkToDo(command); break;
                    case CommandType.MarkInProgress: TaskHandler.MarkInProgress(command); break;
                    case CommandType.MarkDone: TaskHandler.MarkDone(command); break;
                    case CommandType.Help: TaskHandler.PrintHelp(command); break;
                }               
            }
            else
            {
                Console.WriteLine(command.ErrorMessage);
            }

        }
    }   
}
