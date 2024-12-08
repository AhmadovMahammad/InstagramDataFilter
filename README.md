# Instagram Data Parser

This tool is designed to parse recent follow requests from a JSON file and convert the data into a human-readable format. 
The program is currently tailored for handling **recent follow requests** in a specific JSON structure but is planned to be generalized to handle all kinds of JSON data.

## Requirements

- .NET (any version compatible with the application).
- A JSON file formatted similarly to the expected structure for recent follow requests.

## Usage

1. **Build the project**:
   - Open a terminal and navigate to the project directory.
   - Run the following command to build the project:
     ```bash
     dotnet build
     ```

2. **Run the application**:
   - After building, navigate to the `bin/Debug/net8.0/` (or the appropriate folder for your framework version) directory.
   - Run the executable using PowerShell, passing the path to your JSON file as an argument:
     ```bash
     .\RequestParser.exe "C:\path\to\your\recent_follow_requests.json"
     ```

   This will process the JSON file and generate a human-readable text file, which will then be opened using PowerShell.

## Future Enhancements

- Support for various types of JSON data (not just recent follow requests).
- More customization options for output format.
