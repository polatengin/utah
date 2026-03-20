using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

public class CompletionHandler : ICompletionHandler
{
  public CompletionRegistrationOptions GetRegistrationOptions(CompletionCapability capability, ClientCapabilities clientCapabilities)
  {
    return new CompletionRegistrationOptions
    {
      DocumentSelector = new TextDocumentSelector(new TextDocumentFilter
      {
        Language = "utah",
        Pattern = "**/*.shx"
      }),
      TriggerCharacters = new Container<string>("."),
      ResolveProvider = false
    };
  }

  public async Task<CompletionList> Handle(CompletionParams request, CancellationToken cancellationToken)
  {
    var completionItems = new List<CompletionItem>();

    bool isDotTriggered = request.Context?.TriggerCharacter == ".";

    if (isDotTriggered)
    {
      var context = await AnalyzeCompletionContextAsync(request, cancellationToken);

      var namespaceCompletions = context switch
      {
        "console" => GetConsoleCompletions(),
        "fs" => GetFsCompletions(),
        "web" => GetWebCompletions(),
        "json" => GetJsonCompletions(),
        "yaml" => GetYamlCompletions(),
        "validate" => GetValidateCompletions(),
        "process" => GetProcessCompletions(),
        "os" => GetOsCompletions(),
        "git" => GetGitCompletions(),
        "utility" => GetUtilityCompletions(),
        "timer" => GetTimerCompletions(),
        "system" => GetSystemCompletions(),
        "string" => GetStringNamespaceCompletions(),
        "ssh" => GetSshNamespaceCompletions(),
        "args" => GetArgsCompletions(),
        "script" => GetScriptCompletions(),
        "template" => GetTemplateCompletions(),
        "scheduler" => GetSchedulerCompletions(),
        "array" => GetArrayCompletions(),
        "math" => GetMathCompletions(),
        "date" => GetDateCompletions(),
        _ => null
      };

      if (namespaceCompletions != null)
      {
        completionItems.AddRange(namespaceCompletions);
      }
      else if (context.StartsWith("conn") || context.StartsWith("connection") || context.EndsWith("Conn") || context.EndsWith("Connection"))
      {
        completionItems.AddRange(GetSshConnectionCompletions());
      }
    }
    else
    {
      completionItems.AddRange(GetKeywordCompletions());
      completionItems.AddRange(GetNamespaceCompletions());
    }

    return new CompletionList(completionItems);
  }

  private async Task<string> AnalyzeCompletionContextAsync(CompletionParams request, CancellationToken cancellationToken)
  {
    try
    {
      var documentPath = request.TextDocument.Uri.GetFileSystemPath();
      if (!File.Exists(documentPath))
      {
        return "";
      }

      var documentText = await File.ReadAllTextAsync(documentPath, cancellationToken);
      var lines = documentText.Split('\n');

      if (request.Position.Line >= lines.Length)
      {
        return "";
      }

      var line = lines[(int)request.Position.Line];
      if (request.Position.Character > line.Length)
      {
        return "";
      }

      var textBeforeCursor = line.Substring(0, (int)request.Position.Character);

      var match = System.Text.RegularExpressions.Regex.Match(textBeforeCursor, @"(\w+)\.$");
      return match.Success ? match.Groups[1].Value : "";
    }
    catch
    {
      return "";
    }
  }

  public static List<CompletionItem> GetConsoleCompletions()
  {
    return new List<CompletionItem>
    {
      M("log", "log(message: string)", "Print a message to the console"),
      M("clear", "clear()", "Clear the console screen"),
      M("isSudo", "isSudo(): boolean", "Check if the script is running with sudo privileges"),
      M("isInteractive", "isInteractive(): boolean", "Check if the terminal is interactive"),
      M("getShell", "getShell(): string", "Get the current shell name"),
      M("promptYesNo", "promptYesNo(prompt: string): boolean", "Display a yes/no prompt and return the user's choice"),
      M("promptText", "promptText(prompt: string): string", "Prompt user for text input"),
      M("promptPassword", "promptPassword(prompt: string): string", "Prompt user for password input (hidden)"),
      M("promptNumber", "promptNumber(prompt: string): number", "Prompt user for numeric input"),
      M("promptFile", "promptFile(prompt: string): string", "Prompt user to select a file"),
      M("promptDirectory", "promptDirectory(prompt: string): string", "Prompt user to select a directory"),
      M("showMessage", "showMessage(message: string)", "Show an informational message dialog"),
      M("showInfo", "showInfo(message: string)", "Show an info-level message dialog"),
      M("showWarning", "showWarning(message: string)", "Show a warning message dialog"),
      M("showError", "showError(message: string)", "Show an error message dialog"),
      M("showSuccess", "showSuccess(message: string)", "Show a success message dialog"),
      M("showChoice", "showChoice(prompt: string, options: string[]): string", "Show a single-choice dialog"),
      M("showMultiChoice", "showMultiChoice(prompt: string, options: string[]): string[]", "Show a multi-choice dialog"),
      M("showConfirm", "showConfirm(prompt: string): boolean", "Show a confirmation dialog"),
      M("showProgress", "showProgress(message: string)", "Show a progress indicator"),
    };
  }

  public static List<CompletionItem> GetFsCompletions()
  {
    return new List<CompletionItem>
    {
      M("exists", "exists(path: string): boolean", "Check if a file or directory exists"),
      M("readFile", "readFile(path: string): string", "Read the contents of a file"),
      M("writeFile", "writeFile(path: string, content: string)", "Write content to a file"),
      M("copy", "copy(source: string, destination: string)", "Copy a file or directory"),
      M("move", "move(source: string, destination: string)", "Move a file or directory"),
      M("rename", "rename(oldPath: string, newPath: string)", "Rename a file or directory"),
      M("delete", "delete(path: string)", "Delete a file or directory"),
      M("chmod", "chmod(path: string, mode: string)", "Change file permissions"),
      M("chown", "chown(path: string, owner: string)", "Change file ownership"),
      M("find", "find(path: string, pattern: string): string[]", "Find files matching a pattern"),
      M("dirname", "dirname(path: string): string", "Get the directory name from a path"),
      M("fileName", "fileName(path: string): string", "Get the file name from a path"),
      M("extension", "extension(path: string): string", "Get the file extension from a path"),
      M("parentDirName", "parentDirName(path: string): string", "Get the parent directory name"),
      M("createTempFolder", "createTempFolder(): string", "Create a temporary folder and return its path"),
      M("watch", "watch(path: string, callback: function)", "Watch a file or directory for changes"),
    };
  }

  public static List<CompletionItem> GetWebCompletions()
  {
    return new List<CompletionItem>
    {
      M("get", "get(url: string): string", "Send an HTTP GET request"),
      M("post", "post(url: string, body: string): string", "Send an HTTP POST request"),
      M("put", "put(url: string, body: string): string", "Send an HTTP PUT request"),
      M("delete", "delete(url: string): string", "Send an HTTP DELETE request"),
      M("download", "download(url: string, path: string)", "Download a file from a URL"),
      M("speedtest", "speedtest(): string", "Run a network speed test"),
    };
  }

  public static List<CompletionItem> GetJsonCompletions()
  {
    return new List<CompletionItem>
    {
      M("parse", "parse(text: string): object", "Parse a JSON string into an object"),
      M("stringify", "stringify(obj: object): string", "Convert an object to a JSON string"),
      M("isValid", "isValid(text: string): boolean", "Check if a string is valid JSON"),
      M("get", "get(obj: object, path: string): any", "Get a value by JSON path"),
      M("set", "set(obj: object, path: string, value: any): object", "Set a value by JSON path"),
      M("has", "has(obj: object, path: string): boolean", "Check if a JSON path exists"),
      M("delete", "delete(obj: object, path: string): object", "Delete a value by JSON path"),
      M("keys", "keys(obj: object): string[]", "Get all keys from a JSON object"),
      M("values", "values(obj: object): any[]", "Get all values from a JSON object"),
      M("merge", "merge(obj1: object, obj2: object): object", "Deep-merge two JSON objects"),
      M("installDependencies", "installDependencies()", "Install jq if not already present"),
    };
  }

  public static List<CompletionItem> GetYamlCompletions()
  {
    return new List<CompletionItem>
    {
      M("parse", "parse(text: string): object", "Parse a YAML string into an object"),
      M("stringify", "stringify(obj: object): string", "Convert an object to a YAML string"),
      M("isValid", "isValid(text: string): boolean", "Check if a string is valid YAML"),
      M("get", "get(obj: object, path: string): any", "Get a value by YAML path"),
      M("set", "set(obj: object, path: string, value: any): object", "Set a value by YAML path"),
      M("has", "has(obj: object, path: string): boolean", "Check if a YAML path exists"),
      M("delete", "delete(obj: object, path: string): object", "Delete a value by YAML path"),
      M("keys", "keys(obj: object): string[]", "Get all keys from a YAML object"),
      M("values", "values(obj: object): any[]", "Get all values from a YAML object"),
      M("merge", "merge(obj1: object, obj2: object): object", "Deep-merge two YAML objects"),
      M("installDependencies", "installDependencies()", "Install yq if not already present"),
    };
  }

  public static List<CompletionItem> GetValidateCompletions()
  {
    return new List<CompletionItem>
    {
      M("isEmail", "isEmail(value: string): boolean", "Validate if value is a valid email address"),
      M("isURL", "isURL(value: string): boolean", "Validate if value is a valid URL"),
      M("isUUID", "isUUID(value: string): boolean", "Validate if value is a valid UUID"),
      M("isEmpty", "isEmpty(value: string): boolean", "Check if value is empty or whitespace"),
      M("isGreaterThan", "isGreaterThan(value: number, threshold: number): boolean", "Check if value is greater than threshold"),
      M("isLessThan", "isLessThan(value: number, threshold: number): boolean", "Check if value is less than threshold"),
      M("isInRange", "isInRange(value: number, min: number, max: number): boolean", "Check if value is within range"),
      M("isNumeric", "isNumeric(value: string): boolean", "Check if value contains only numeric characters"),
      M("isAlphaNumeric", "isAlphaNumeric(value: string): boolean", "Check if value contains only alphanumeric characters"),
    };
  }

  public static List<CompletionItem> GetProcessCompletions()
  {
    return new List<CompletionItem>
    {
      M("start", "start(command: string): number", "Start a new process and return its PID"),
      M("id", "id(): number", "Get the current process ID"),
      M("elapsedTime", "elapsedTime(): number", "Get elapsed time in seconds since process start"),
      M("cpu", "cpu(): number", "Get CPU usage percentage of the current process"),
      M("memory", "memory(): number", "Get memory usage of the current process"),
      M("command", "command(): string", "Get the command that started the current process"),
      M("status", "status(pid: number): string", "Get the status of a process by PID"),
      M("isRunning", "isRunning(pid: number): boolean", "Check if a process is running"),
      M("waitForExit", "waitForExit(pid: number): number", "Wait for a process to exit and return exit code"),
      M("kill", "kill(pid: number)", "Kill a process by PID"),
    };
  }

  public static List<CompletionItem> GetOsCompletions()
  {
    return new List<CompletionItem>
    {
      M("isInstalled", "isInstalled(command: string): boolean", "Check if a command/program is installed"),
      M("getLinuxVersion", "getLinuxVersion(): string", "Get the Linux distribution version"),
      M("getOS", "getOS(): string", "Get the operating system name"),
    };
  }

  public static List<CompletionItem> GetGitCompletions()
  {
    return new List<CompletionItem>
    {
      M("undoLastCommit", "undoLastCommit()", "Undo the last git commit (soft reset)"),
      M("status", "status(): string", "Get the current git status"),
      M("currentBranch", "currentBranch(): string", "Get the current git branch name"),
      M("isClean", "isClean(): boolean", "Check if working directory is clean"),
      M("resetToCommit", "resetToCommit(commit: string)", "Reset to a specific commit"),
    };
  }

  public static List<CompletionItem> GetUtilityCompletions()
  {
    return new List<CompletionItem>
    {
      M("random", "random(min: number, max: number): number", "Generate a random number within range"),
      M("uuid", "uuid(): string", "Generate a new UUID"),
      M("hash", "hash(value: string, algorithm?: string): string", "Hash a string with specified algorithm"),
      M("base64Encode", "base64Encode(value: string): string", "Encode a string to base64"),
      M("base64Decode", "base64Decode(value: string): string", "Decode a base64 string"),
    };
  }

  public static List<CompletionItem> GetTimerCompletions()
  {
    return new List<CompletionItem>
    {
      M("start", "start(name: string)", "Start a named timer"),
      M("stop", "stop(name: string): number", "Stop a named timer and return elapsed milliseconds"),
      M("current", "current(): number", "Get current timestamp in milliseconds"),
    };
  }

  public static List<CompletionItem> GetSystemCompletions()
  {
    return new List<CompletionItem>
    {
      M("cpuCount", "cpuCount(): number", "Get the number of CPU cores"),
      M("memoryTotal", "memoryTotal(): number", "Get total system memory in bytes"),
      M("memoryUsage", "memoryUsage(): number", "Get current memory usage in bytes"),
    };
  }

  public static List<CompletionItem> GetArgsCompletions()
  {
    return new List<CompletionItem>
    {
      M("has", "has(name: string): boolean", "Check if a named argument was provided"),
      M("get", "get(name: string): string", "Get the value of a named argument"),
      M("all", "all(): string[]", "Get all arguments as an array"),
      M("define", "define(name: string, options: object)", "Define an expected argument with type and default"),
    };
  }

  public static List<CompletionItem> GetScriptCompletions()
  {
    return new List<CompletionItem>
    {
      M("enableDebug", "enableDebug()", "Enable shell debugging (set -x)"),
      M("disableDebug", "disableDebug()", "Disable shell debugging (set +x)"),
      M("disableGlobbing", "disableGlobbing()", "Disable filename globbing (set -f)"),
      M("enableGlobbing", "enableGlobbing()", "Enable filename globbing (set +f)"),
      M("exitOnError", "exitOnError()", "Exit script on any command failure (set -e)"),
      M("continueOnError", "continueOnError()", "Continue script execution on command failure (set +e)"),
      M("description", "description(text: string)", "Set the script description for help output"),
    };
  }

  public static List<CompletionItem> GetTemplateCompletions()
  {
    return new List<CompletionItem>
    {
      M("update", "update(templatePath: string, outputPath: string, vars: object)", "Apply template variable substitution"),
    };
  }

  public static List<CompletionItem> GetSchedulerCompletions()
  {
    return new List<CompletionItem>
    {
      M("cron", "cron(expression: string, command: string)", "Schedule a command with a cron expression"),
    };
  }

  public static List<CompletionItem> GetArrayCompletions()
  {
    return new List<CompletionItem>
    {
      M("join", "join(arr: any[], separator: string): string", "Join array elements into a string"),
      M("sort", "sort(arr: any[]): any[]", "Sort array elements"),
      M("reverse", "reverse(arr: any[]): any[]", "Reverse array element order"),
      M("shuffle", "shuffle(arr: any[]): any[]", "Randomly shuffle array elements"),
      M("unique", "unique(arr: any[]): any[]", "Remove duplicate elements from array"),
      M("merge", "merge(arr1: any[], arr2: any[]): any[]", "Merge two arrays together"),
      M("contains", "contains(arr: any[], value: any): boolean", "Check if array contains value"),
      M("isEmpty", "isEmpty(arr: any[]): boolean", "Check if array is empty"),
      M("forEach", "forEach(arr: any[], callback: function)", "Execute callback for each element"),
      M("map", "map(arr: any[], callback: function): any[]", "Transform each element with callback"),
      M("filter", "filter(arr: any[], callback: function): any[]", "Filter elements by callback"),
      M("reduce", "reduce(arr: any[], callback: function, initial: any): any", "Reduce array to single value"),
      M("find", "find(arr: any[], callback: function): any", "Find first element matching callback"),
      M("some", "some(arr: any[], callback: function): boolean", "Check if any element matches callback"),
      M("every", "every(arr: any[], callback: function): boolean", "Check if all elements match callback"),
    };
  }

  public static List<CompletionItem> GetMathCompletions()
  {
    return new List<CompletionItem>
    {
      M("abs", "abs(value: number): number", "Get the absolute value"),
      M("ceil", "ceil(value: number): number", "Round up to nearest integer"),
      M("floor", "floor(value: number): number", "Round down to nearest integer"),
      M("round", "round(value: number): number", "Round to nearest integer"),
      M("min", "min(a: number, b: number): number", "Get the smaller of two values"),
      M("max", "max(a: number, b: number): number", "Get the larger of two values"),
      M("pow", "pow(base: number, exp: number): number", "Raise base to a power"),
      M("sqrt", "sqrt(value: number): number", "Get the square root"),
    };
  }

  public static List<CompletionItem> GetStringNamespaceCompletions()
  {
    return new List<CompletionItem>
    {
      M("length", "length(value: string): number", "Get the length of a string"),
      M("trim", "trim(value: string): string", "Remove whitespace from both ends of a string"),
      M("isEmpty", "isEmpty(value: string): boolean", "Check if string is empty or contains only whitespace"),
      M("toUpperCase", "toUpperCase(value: string): string", "Convert string to uppercase"),
      M("toLowerCase", "toLowerCase(value: string): string", "Convert string to lowercase"),
      M("capitalize", "capitalize(value: string): string", "Capitalize the first letter of a string"),
      M("startsWith", "startsWith(value: string, prefix: string): boolean", "Check if string starts with the specified prefix"),
      M("endsWith", "endsWith(value: string, suffix: string): boolean", "Check if string ends with the specified suffix"),
      M("contains", "contains(value: string, searchValue: string): boolean", "Check if string contains the specified substring"),
      M("indexOf", "indexOf(value: string, searchValue: string): number", "Find the index of the first occurrence of a substring"),
      M("substring", "substring(value: string, start: number, length?: number): string", "Extract a substring starting at the specified index"),
      M("slice", "slice(value: string, start: number, end?: number): string", "Extract a section of the string between start and end indices"),
      M("replace", "replace(value: string, searchValue: string, replaceValue: string): string", "Replace the first occurrence of a substring"),
      M("replaceAll", "replaceAll(value: string, searchValue: string, replaceValue: string): string", "Replace all occurrences of a substring"),
      M("split", "split(value: string, separator: string): string[]", "Split a string into an array using the specified separator"),
      M("padStart", "padStart(value: string, length: number, pad?: string): string", "Pad the string from the start to reach the specified length"),
      M("padEnd", "padEnd(value: string, length: number, pad?: string): string", "Pad the string from the end to reach the specified length"),
      M("repeat", "repeat(value: string, count: number): string", "Repeat the string the specified number of times"),
    };
  }

  public static List<CompletionItem> GetSshNamespaceCompletions()
  {
    return new List<CompletionItem>
    {
      M("connect", "connect(host: string, options?: object): object", "Establish SSH connection with optional async support"),
    };
  }

  private static List<CompletionItem> GetSshConnectionCompletions()
  {
    return new List<CompletionItem>
    {
      P("connected", "connected: boolean", "Connection status - true if connected, false otherwise"),
      P("host", "host: string", "The target hostname or IP address"),
      P("port", "port: string", "The SSH port (default: '22')"),
      P("username", "username: string", "The SSH username for authentication"),
      P("authMethod", "authMethod: string", "Authentication method: 'config', 'key', or 'password'"),
      P("async", "async: boolean", "Whether the connection is persistent (async: true) or one-time"),
      M("execute", "execute(command: string): string", "Execute a command on the remote server and return the output"),
      M("upload", "upload(localPath: string, remotePath: string): boolean", "Upload a file to the remote server, returns true on success"),
      M("download", "download(remotePath: string, localPath: string): boolean", "Download a file from the remote server"),
    };
  }

  private static List<CompletionItem> GetKeywordCompletions()
  {
    return new List<CompletionItem>
    {
      K("let", "let variable: type = value", "Declare a mutable variable"),
      K("const", "const variable: type = value", "Declare a constant variable"),
      K("function", "function name(params) { ... }", "Declare a function"),
      K("if", "if (condition) { ... }", "Conditional statement"),
      K("else", "else { ... }", "Alternative branch for if statement"),
      K("for", "for (init; condition; increment) { ... }", "For loop"),
      K("while", "while (condition) { ... }", "While loop"),
      K("return", "return value", "Return a value from function"),
      K("break", "break", "Break out of loop or switch"),
      K("continue", "continue", "Skip to next iteration of loop"),
      K("try", "try { ... } catch (e) { ... }", "Try-catch error handling"),
      K("catch", "catch (error) { ... }", "Catch block for error handling"),
      K("defer", "defer expression", "Execute expression when scope exits"),
      K("import", "import \"path/to/file.shx\"", "Import another shx file"),
      K("switch", "switch (expression) { ... }", "Switch statement"),
      K("case", "case value: ...", "Case clause in switch statement"),
      K("default", "default: ...", "Default clause in switch statement"),
      K("parallel", "parallel { ... }", "Execute functions in parallel"),
      K("record", "record Name { ... }", "Define a record type"),
      K("interface", "interface Name { ... }", "Define an interface type"),
      K("true", "true", "Boolean true value"),
      K("false", "false", "Boolean false value"),
      K("exit", "exit(code?: number)", "Exit the script with optional code"),
    };
  }

  private static List<CompletionItem> GetNamespaceCompletions()
  {
    return new List<CompletionItem>
    {
      N("console", "Console operations namespace"),
      N("fs", "File system operations namespace"),
      N("web", "HTTP request operations namespace"),
      N("json", "JSON parsing and manipulation namespace"),
      N("yaml", "YAML parsing and manipulation namespace"),
      N("validate", "Input validation namespace"),
      N("process", "Process management namespace"),
      N("os", "Operating system operations namespace"),
      N("git", "Git operations namespace"),
      N("utility", "Utility functions namespace"),
      N("timer", "Timer operations namespace"),
      N("system", "System information namespace"),
      N("string", "String operations namespace"),
      N("math", "Math operations namespace"),
      N("ssh", "SSH remote connection namespace"),
      N("args", "Script argument handling namespace"),
      N("script", "Script control operations namespace"),
      N("template", "Template variable substitution namespace"),
      N("scheduler", "Task scheduling namespace"),
      N("array", "Array operations namespace"),
      N("date", "Date and time operations namespace"),
    };
  }

  public static List<CompletionItem> GetDateCompletions()
  {
    return new List<CompletionItem>
    {
      M("now", "now(): number", "Get the current Unix timestamp in seconds"),
      M("nowMillis", "nowMillis(): number", "Get the current Unix timestamp in milliseconds"),
      M("format", "format(timestamp?: number, format?: string): string", "Format a timestamp into a human-readable date string"),
      M("parse", "parse(dateString: string, format?: string): number", "Parse a date string into a Unix timestamp"),
      M("diff", "diff(ts1: number, ts2: number, unit?: string): number", "Calculate the difference between two timestamps"),
      M("add", "add(timestamp: number, amount: number, unit: string): number", "Add time to a timestamp"),
      M("subtract", "subtract(timestamp: number, amount: number, unit: string): number", "Subtract time from a timestamp"),
      M("dayOfWeek", "dayOfWeek(timestamp?: number): string", "Get the day of the week name for a timestamp"),
    };
  }

  private static CompletionItem M(string label, string detail, string documentation) =>
    new() { Label = label, Kind = CompletionItemKind.Method, Detail = detail, Documentation = documentation };

  private static CompletionItem P(string label, string detail, string documentation) =>
    new() { Label = label, Kind = CompletionItemKind.Property, Detail = detail, Documentation = documentation };

  private static CompletionItem K(string label, string detail, string documentation) =>
    new() { Label = label, Kind = CompletionItemKind.Keyword, Detail = detail, Documentation = documentation };

  private static CompletionItem N(string label, string documentation) =>
    new() { Label = label, Kind = CompletionItemKind.Module, Detail = label, Documentation = documentation };
}
