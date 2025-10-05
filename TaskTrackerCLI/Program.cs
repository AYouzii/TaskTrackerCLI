// using System;
// using System.IO;

namespace TaskTrackerCLI
{
    class Program
    {
        public static void Main(string[] args)
        {
            var command = InputHandler.Parse(args);
            TaskHandler.AddTask(command);
            Console.WriteLine(command.Description);
        }
    }   
}
