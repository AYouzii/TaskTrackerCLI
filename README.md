# Task Tracker CLI

A simple command-line tool to manage your tasks.

## Usage

```text
TaskTrackerCLI add <description>                      Add a new task
TaskTrackerCLI update <id> <description>              Update a task's description
TaskTrackerCLI delete <id1> [id2] [id3] ...           Delete one or more tasks by ID
TaskTrackerCLI mark-todo <id1> [id2] [id3] ...        Mark tasks as todo
TaskTrackerCLI mark-in-progress <id1> [id2] [id3] ... Mark tasks as in-progress
TaskTrackerCLI mark-done <id1> [id2] [id3] ...        Mark tasks as done
TaskTrackerCLI list [status]                          List tasks (todo/in-progress/done). Lists all if no status is given
TaskTrackerCLI help                                   Show this help message
```

## Examples

```bash
TaskTrackerCLI add "Buy groceries"
TaskTrackerCLI update 1 "Buy milk and bread"
TaskTrackerCLI delete 2 3
TaskTrackerCLI mark-done 1
TaskTrackerCLI list todo
TaskTrackerCLI list
TaskTrackerCLI help
```

## Data Storage

Tasks are saved automatically in a `data.json` file in the same directory.
