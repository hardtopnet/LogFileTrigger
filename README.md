# LogFileTrigger

LogFileTrigger is a lightweight background application that monitors a file for specific entries, and triggers a specified command if the entry is detected.

## Features

- very lightweight
- configuration in JSON file
- specify a file, a pattern to look for, and a command to execute
- for Windows : provides a .bat file to launch in background from links, task scheduler, ...

## Installation

1. Clone the repository:
    ```bash
    git clone https://github.com/yourusername/LogFileTrigger.git
    ```
2. Navigate to the project directory:
    ```bash
    cd LogFileTrigger
    ```
3. The program doesn't use external dependencies. Just in case you can restore
    ```bash
    dotnet restore
    ```

## Usage

```bash
LogFileTrigger
```

### Settings

```json
{
  "LogFilePath": "<path_to_the_file_to_monitor>",
  "Pattern": "<pattern_to_listen_for>",
  "Command": "<command_to_execute_when_pattern_matched>"
}
```
