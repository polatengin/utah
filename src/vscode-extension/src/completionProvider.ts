import * as vscode from 'vscode';

interface NamespaceInfo {
  description: string;
  members: MemberInfo[];
}

interface MemberInfo {
  label: string;
  detail: string;
  documentation: string;
  kind: vscode.CompletionItemKind;
}

function M(label: string, detail: string, documentation: string): MemberInfo {
  return { label, detail, documentation, kind: vscode.CompletionItemKind.Method };
}

function P(label: string, detail: string, documentation: string): MemberInfo {
  return { label, detail, documentation, kind: vscode.CompletionItemKind.Property };
}

const namespaces: Record<string, NamespaceInfo> = {
  console: {
    description: 'Console operations namespace',
    members: [
      M('log', 'log(message: string)', 'Print a message to the console'),
      M('clear', 'clear()', 'Clear the console screen'),
      M('isSudo', 'isSudo(): boolean', 'Check if the script is running with sudo privileges'),
      M('isInteractive', 'isInteractive(): boolean', 'Check if the terminal is interactive'),
      M('getShell', 'getShell(): string', 'Get the current shell name'),
      M('promptYesNo', 'promptYesNo(prompt: string): boolean', 'Display a yes/no prompt and return the user\'s choice'),
      M('promptText', 'promptText(prompt: string): string', 'Prompt user for text input'),
      M('promptPassword', 'promptPassword(prompt: string): string', 'Prompt user for password input (hidden)'),
      M('promptNumber', 'promptNumber(prompt: string): number', 'Prompt user for numeric input'),
      M('promptFile', 'promptFile(prompt: string): string', 'Prompt user to select a file'),
      M('promptDirectory', 'promptDirectory(prompt: string): string', 'Prompt user to select a directory'),
      M('showMessage', 'showMessage(message: string)', 'Show an informational message dialog'),
      M('showInfo', 'showInfo(message: string)', 'Show an info-level message dialog'),
      M('showWarning', 'showWarning(message: string)', 'Show a warning message dialog'),
      M('showError', 'showError(message: string)', 'Show an error message dialog'),
      M('showSuccess', 'showSuccess(message: string)', 'Show a success message dialog'),
      M('showChoice', 'showChoice(prompt: string, options: string[]): string', 'Show a single-choice dialog'),
      M('showMultiChoice', 'showMultiChoice(prompt: string, options: string[]): string[]', 'Show a multi-choice dialog'),
      M('showConfirm', 'showConfirm(prompt: string): boolean', 'Show a confirmation dialog'),
      M('showProgress', 'showProgress(message: string)', 'Show a progress indicator'),
    ],
  },
  fs: {
    description: 'File system operations namespace',
    members: [
      M('exists', 'exists(path: string): boolean', 'Check if a file or directory exists'),
      M('readFile', 'readFile(path: string): string', 'Read the contents of a file'),
      M('writeFile', 'writeFile(path: string, content: string)', 'Write content to a file'),
      M('copy', 'copy(source: string, destination: string)', 'Copy a file or directory'),
      M('move', 'move(source: string, destination: string)', 'Move a file or directory'),
      M('rename', 'rename(oldPath: string, newPath: string)', 'Rename a file or directory'),
      M('delete', 'delete(path: string)', 'Delete a file or directory'),
      M('chmod', 'chmod(path: string, mode: string)', 'Change file permissions'),
      M('chown', 'chown(path: string, owner: string)', 'Change file ownership'),
      M('find', 'find(path: string, pattern: string): string[]', 'Find files matching a pattern'),
      M('dirname', 'dirname(path: string): string', 'Get the directory name from a path'),
      M('fileName', 'fileName(path: string): string', 'Get the file name from a path'),
      M('extension', 'extension(path: string): string', 'Get the file extension from a path'),
      M('parentDirName', 'parentDirName(path: string): string', 'Get the parent directory name'),
      M('createTempFolder', 'createTempFolder(): string', 'Create a temporary folder and return its path'),
      M('watch', 'watch(path: string, callback: function)', 'Watch a file or directory for changes'),
    ],
  },
  web: {
    description: 'HTTP request operations namespace',
    members: [
      M('get', 'get(url: string): string', 'Send an HTTP GET request'),
      M('post', 'post(url: string, body: string): string', 'Send an HTTP POST request'),
      M('put', 'put(url: string, body: string): string', 'Send an HTTP PUT request'),
      M('delete', 'delete(url: string): string', 'Send an HTTP DELETE request'),
      M('download', 'download(url: string, path: string)', 'Download a file from a URL'),
      M('speedtest', 'speedtest(): string', 'Run a network speed test'),
    ],
  },
  json: {
    description: 'JSON parsing and manipulation namespace',
    members: [
      M('parse', 'parse(text: string): object', 'Parse a JSON string into an object'),
      M('stringify', 'stringify(obj: object): string', 'Convert an object to a JSON string'),
      M('isValid', 'isValid(text: string): boolean', 'Check if a string is valid JSON'),
      M('get', 'get(obj: object, path: string): any', 'Get a value by JSON path'),
      M('set', 'set(obj: object, path: string, value: any): object', 'Set a value by JSON path'),
      M('has', 'has(obj: object, path: string): boolean', 'Check if a JSON path exists'),
      M('delete', 'delete(obj: object, path: string): object', 'Delete a value by JSON path'),
      M('keys', 'keys(obj: object): string[]', 'Get all keys from a JSON object'),
      M('values', 'values(obj: object): any[]', 'Get all values from a JSON object'),
      M('merge', 'merge(obj1: object, obj2: object): object', 'Deep-merge two JSON objects'),
      M('installDependencies', 'installDependencies()', 'Install jq if not already present'),
    ],
  },
  yaml: {
    description: 'YAML parsing and manipulation namespace',
    members: [
      M('parse', 'parse(text: string): object', 'Parse a YAML string into an object'),
      M('stringify', 'stringify(obj: object): string', 'Convert an object to a YAML string'),
      M('isValid', 'isValid(text: string): boolean', 'Check if a string is valid YAML'),
      M('get', 'get(obj: object, path: string): any', 'Get a value by YAML path'),
      M('set', 'set(obj: object, path: string, value: any): object', 'Set a value by YAML path'),
      M('has', 'has(obj: object, path: string): boolean', 'Check if a YAML path exists'),
      M('delete', 'delete(obj: object, path: string): object', 'Delete a value by YAML path'),
      M('keys', 'keys(obj: object): string[]', 'Get all keys from a YAML object'),
      M('values', 'values(obj: object): any[]', 'Get all values from a YAML object'),
      M('merge', 'merge(obj1: object, obj2: object): object', 'Deep-merge two YAML objects'),
      M('installDependencies', 'installDependencies()', 'Install yq if not already present'),
    ],
  },
  validate: {
    description: 'Input validation namespace',
    members: [
      M('isEmail', 'isEmail(value: string): boolean', 'Validate if value is a valid email address'),
      M('isURL', 'isURL(value: string): boolean', 'Validate if value is a valid URL'),
      M('isUUID', 'isUUID(value: string): boolean', 'Validate if value is a valid UUID'),
      M('isEmpty', 'isEmpty(value: string): boolean', 'Check if value is empty or whitespace'),
      M('isGreaterThan', 'isGreaterThan(value: number, threshold: number): boolean', 'Check if value is greater than threshold'),
      M('isLessThan', 'isLessThan(value: number, threshold: number): boolean', 'Check if value is less than threshold'),
      M('isInRange', 'isInRange(value: number, min: number, max: number): boolean', 'Check if value is within range'),
      M('isNumeric', 'isNumeric(value: string): boolean', 'Check if value contains only numeric characters'),
      M('isAlphaNumeric', 'isAlphaNumeric(value: string): boolean', 'Check if value contains only alphanumeric characters'),
    ],
  },
  process: {
    description: 'Process management namespace',
    members: [
      M('start', 'start(command: string): number', 'Start a new process and return its PID'),
      M('id', 'id(): number', 'Get the current process ID'),
      M('elapsedTime', 'elapsedTime(): number', 'Get elapsed time in seconds since process start'),
      M('cpu', 'cpu(): number', 'Get CPU usage percentage of the current process'),
      M('memory', 'memory(): number', 'Get memory usage of the current process'),
      M('command', 'command(): string', 'Get the command that started the current process'),
      M('status', 'status(pid: number): string', 'Get the status of a process by PID'),
      M('isRunning', 'isRunning(pid: number): boolean', 'Check if a process is running'),
      M('waitForExit', 'waitForExit(pid: number): number', 'Wait for a process to exit and return exit code'),
      M('kill', 'kill(pid: number)', 'Kill a process by PID'),
    ],
  },
  os: {
    description: 'Operating system operations namespace',
    members: [
      M('isInstalled', 'isInstalled(command: string): boolean', 'Check if a command/program is installed'),
      M('getLinuxVersion', 'getLinuxVersion(): string', 'Get the Linux distribution version'),
      M('getOS', 'getOS(): string', 'Get the operating system name'),
    ],
  },
  git: {
    description: 'Git operations namespace',
    members: [
      M('undoLastCommit', 'undoLastCommit()', 'Undo the last git commit (soft reset)'),
      M('status', 'status(): string', 'Get the current git status'),
      M('currentBranch', 'currentBranch(): string', 'Get the current git branch name'),
      M('isClean', 'isClean(): boolean', 'Check if working directory is clean'),
      M('resetToCommit', 'resetToCommit(commit: string)', 'Reset to a specific commit'),
    ],
  },
  docker: {
    description: 'Docker container and image management namespace',
    members: [
      M('run', 'run(image: string, name?: string, ports?: string, volumes?: string): string', 'Run a Docker container'),
      M('stop', 'stop(container: string)', 'Stop a running container'),
      M('remove', 'remove(container: string)', 'Remove a container'),
      M('restart', 'restart(container: string)', 'Restart a container'),
      M('logs', 'logs(container: string): string', 'Get container logs'),
      M('exec', 'exec(container: string, command: string): string', 'Execute a command in a running container'),
      M('isRunning', 'isRunning(container: string): boolean', 'Check if a container is running'),
      M('list', 'list(): string', 'List running containers'),
      M('build', 'build(tag: string, path?: string)', 'Build a Docker image'),
      M('pull', 'pull(image: string)', 'Pull an image from registry'),
      M('push', 'push(image: string)', 'Push an image to registry'),
      M('removeImage', 'removeImage(image: string)', 'Remove a Docker image'),
      M('imageExists', 'imageExists(image: string): boolean', 'Check if an image exists locally'),
    ],
  },
  utility: {
    description: 'Utility functions namespace',
    members: [
      M('random', 'random(min: number, max: number): number', 'Generate a random number within range'),
      M('uuid', 'uuid(): string', 'Generate a new UUID'),
      M('hash', 'hash(value: string, algorithm?: string): string', 'Hash a string with specified algorithm'),
      M('base64Encode', 'base64Encode(value: string): string', 'Encode a string to base64'),
      M('base64Decode', 'base64Decode(value: string): string', 'Decode a base64 string'),
    ],
  },
  timer: {
    description: 'Timer operations namespace',
    members: [
      M('start', 'start(name: string)', 'Start a named timer'),
      M('stop', 'stop(name: string): number', 'Stop a named timer and return elapsed milliseconds'),
      M('current', 'current(): number', 'Get current timestamp in milliseconds'),
    ],
  },
  system: {
    description: 'System information namespace',
    members: [
      M('cpuCount', 'cpuCount(): number', 'Get the number of CPU cores'),
      M('memoryTotal', 'memoryTotal(): number', 'Get total system memory in bytes'),
      M('memoryUsage', 'memoryUsage(): number', 'Get current memory usage in bytes'),
    ],
  },
  string: {
    description: 'String operations namespace',
    members: [
      M('length', 'length(value: string): number', 'Get the length of a string'),
      M('trim', 'trim(value: string): string', 'Remove whitespace from both ends of a string'),
      M('isEmpty', 'isEmpty(value: string): boolean', 'Check if string is empty or contains only whitespace'),
      M('toUpperCase', 'toUpperCase(value: string): string', 'Convert string to uppercase'),
      M('toLowerCase', 'toLowerCase(value: string): string', 'Convert string to lowercase'),
      M('capitalize', 'capitalize(value: string): string', 'Capitalize the first letter of a string'),
      M('startsWith', 'startsWith(value: string, prefix: string): boolean', 'Check if string starts with the specified prefix'),
      M('endsWith', 'endsWith(value: string, suffix: string): boolean', 'Check if string ends with the specified suffix'),
      M('contains', 'contains(value: string, searchValue: string): boolean', 'Check if string contains the specified substring'),
      M('indexOf', 'indexOf(value: string, searchValue: string): number', 'Find the index of the first occurrence of a substring'),
      M('substring', 'substring(value: string, start: number, length?: number): string', 'Extract a substring starting at the specified index'),
      M('slice', 'slice(value: string, start: number, end?: number): string', 'Extract a section of the string between start and end indices'),
      M('replace', 'replace(value: string, searchValue: string, replaceValue: string): string', 'Replace the first occurrence of a substring'),
      M('replaceAll', 'replaceAll(value: string, searchValue: string, replaceValue: string): string', 'Replace all occurrences of a substring'),
      M('split', 'split(value: string, separator: string): string[]', 'Split a string into an array using the specified separator'),
      M('padStart', 'padStart(value: string, length: number, pad?: string): string', 'Pad the string from the start to reach the specified length'),
      M('padEnd', 'padEnd(value: string, length: number, pad?: string): string', 'Pad the string from the end to reach the specified length'),
      M('repeat', 'repeat(value: string, count: number): string', 'Repeat the string the specified number of times'),
    ],
  },
  math: {
    description: 'Math operations namespace',
    members: [
      M('abs', 'abs(value: number): number', 'Get the absolute value'),
      M('ceil', 'ceil(value: number): number', 'Round up to nearest integer'),
      M('floor', 'floor(value: number): number', 'Round down to nearest integer'),
      M('round', 'round(value: number): number', 'Round to nearest integer'),
      M('min', 'min(a: number, b: number): number', 'Get the smaller of two values'),
      M('max', 'max(a: number, b: number): number', 'Get the larger of two values'),
      M('pow', 'pow(base: number, exp: number): number', 'Raise base to a power'),
      M('sqrt', 'sqrt(value: number): number', 'Get the square root'),
    ],
  },
  ssh: {
    description: 'SSH remote connection namespace',
    members: [
      M('connect', 'connect(host: string, options?: object): object', 'Establish SSH connection with optional async support'),
    ],
  },
  args: {
    description: 'Script argument handling namespace',
    members: [
      M('has', 'has(name: string): boolean', 'Check if a named argument was provided'),
      M('get', 'get(name: string): string', 'Get the value of a named argument'),
      M('all', 'all(): string[]', 'Get all arguments as an array'),
      M('define', 'define(name: string, options: object)', 'Define an expected argument with type and default'),
    ],
  },
  script: {
    description: 'Script control operations namespace',
    members: [
      M('enableDebug', 'enableDebug()', 'Enable shell debugging (set -x)'),
      M('disableDebug', 'disableDebug()', 'Disable shell debugging (set +x)'),
      M('disableGlobbing', 'disableGlobbing()', 'Disable filename globbing (set -f)'),
      M('enableGlobbing', 'enableGlobbing()', 'Enable filename globbing (set +f)'),
      M('exitOnError', 'exitOnError()', 'Exit script on any command failure (set -e)'),
      M('continueOnError', 'continueOnError()', 'Continue script execution on command failure (set +e)'),
      M('description', 'description(text: string)', 'Set the script description for help output'),
    ],
  },
  template: {
    description: 'Template variable substitution namespace',
    members: [
      M('update', 'update(templatePath: string, outputPath: string, vars: object)', 'Apply template variable substitution'),
    ],
  },
  scheduler: {
    description: 'Task scheduling namespace',
    members: [
      M('cron', 'cron(expression: string, command: string)', 'Schedule a command with a cron expression'),
    ],
  },
  array: {
    description: 'Array operations namespace',
    members: [
      M('join', 'join(arr: any[], separator: string): string', 'Join array elements into a string'),
      M('sort', 'sort(arr: any[]): any[]', 'Sort array elements'),
      M('reverse', 'reverse(arr: any[]): any[]', 'Reverse array element order'),
      M('shuffle', 'shuffle(arr: any[]): any[]', 'Randomly shuffle array elements'),
      M('unique', 'unique(arr: any[]): any[]', 'Remove duplicate elements from array'),
      M('merge', 'merge(arr1: any[], arr2: any[]): any[]', 'Merge two arrays together'),
      M('contains', 'contains(arr: any[], value: any): boolean', 'Check if array contains value'),
      M('isEmpty', 'isEmpty(arr: any[]): boolean', 'Check if array is empty'),
      M('forEach', 'forEach(arr: any[], callback: function)', 'Execute callback for each element'),
      M('map', 'map(arr: any[], callback: function): any[]', 'Transform each element with callback'),
      M('filter', 'filter(arr: any[], callback: function): any[]', 'Filter elements by callback'),
      M('reduce', 'reduce(arr: any[], callback: function, initial: any): any', 'Reduce array to single value'),
      M('find', 'find(arr: any[], callback: function): any', 'Find first element matching callback'),
      M('some', 'some(arr: any[], callback: function): boolean', 'Check if any element matches callback'),
      M('every', 'every(arr: any[], callback: function): boolean', 'Check if all elements match callback'),
    ],
  },
  date: {
    description: 'Date and time operations namespace',
    members: [
      M('now', 'now(): number', 'Get the current Unix timestamp in seconds'),
      M('nowMillis', 'nowMillis(): number', 'Get the current Unix timestamp in milliseconds'),
      M('format', 'format(timestamp?: number, format?: string): string', 'Format a timestamp into a human-readable date string'),
      M('parse', 'parse(dateString: string, format?: string): number', 'Parse a date string into a Unix timestamp'),
      M('diff', 'diff(ts1: number, ts2: number, unit?: string): number', 'Calculate the difference between two timestamps'),
      M('add', 'add(timestamp: number, amount: number, unit: string): number', 'Add time to a timestamp'),
      M('subtract', 'subtract(timestamp: number, amount: number, unit: string): number', 'Subtract time from a timestamp'),
      M('dayOfWeek', 'dayOfWeek(timestamp?: number): string', 'Get the day of the week name for a timestamp'),
    ],
  },
};

const sshConnectionMembers: MemberInfo[] = [
  P('connected', 'connected: boolean', 'Connection status - true if connected, false otherwise'),
  P('host', 'host: string', 'The target hostname or IP address'),
  P('port', 'port: string', 'The SSH port (default: \'22\')'),
  P('username', 'username: string', 'The SSH username for authentication'),
  P('authMethod', 'authMethod: string', 'Authentication method: \'config\', \'key\', or \'password\''),
  P('async', 'async: boolean', 'Whether the connection is persistent (async: true) or one-time'),
  M('execute', 'execute(command: string): string', 'Execute a command on the remote server and return the output'),
  M('upload', 'upload(localPath: string, remotePath: string): boolean', 'Upload a file to the remote server, returns true on success'),
  M('download', 'download(remotePath: string, localPath: string): boolean', 'Download a file from the remote server'),
];

const keywords = [
  { label: 'let', detail: 'let variable: type = value', documentation: 'Declare a mutable variable' },
  { label: 'const', detail: 'const variable: type = value', documentation: 'Declare a constant variable' },
  { label: 'function', detail: 'function name(params) { ... }', documentation: 'Declare a function' },
  { label: 'if', detail: 'if (condition) { ... }', documentation: 'Conditional statement' },
  { label: 'else', detail: 'else { ... }', documentation: 'Alternative branch for if statement' },
  { label: 'for', detail: 'for (init; condition; increment) { ... }', documentation: 'For loop' },
  { label: 'while', detail: 'while (condition) { ... }', documentation: 'While loop' },
  { label: 'return', detail: 'return value', documentation: 'Return a value from function' },
  { label: 'break', detail: 'break', documentation: 'Break out of loop or switch' },
  { label: 'continue', detail: 'continue', documentation: 'Skip to next iteration of loop' },
  { label: 'try', detail: 'try { ... } catch (e) { ... }', documentation: 'Try-catch error handling' },
  { label: 'catch', detail: 'catch (error) { ... }', documentation: 'Catch block for error handling' },
  { label: 'defer', detail: 'defer expression', documentation: 'Execute expression when scope exits' },
  { label: 'import', detail: 'import "path/to/file.shx"', documentation: 'Import another shx file' },
  { label: 'switch', detail: 'switch (expression) { ... }', documentation: 'Switch statement' },
  { label: 'case', detail: 'case value: ...', documentation: 'Case clause in switch statement' },
  { label: 'default', detail: 'default: ...', documentation: 'Default clause in switch statement' },
  { label: 'parallel', detail: 'parallel { ... }', documentation: 'Execute functions in parallel' },
  { label: 'record', detail: 'record Name { ... }', documentation: 'Define a record type' },
  { label: 'interface', detail: 'interface Name { ... }', documentation: 'Define an interface type' },
  { label: 'true', detail: 'true', documentation: 'Boolean true value' },
  { label: 'false', detail: 'false', documentation: 'Boolean false value' },
  { label: 'exit', detail: 'exit(code?: number)', documentation: 'Exit the script with optional code' },
];

interface DocumentVariable {
  name: string;
  type: string;
  isConst: boolean;
}

interface DocumentFunction {
  name: string;
  params: string;
  returnType: string;
}

function extractVariables(document: vscode.TextDocument): DocumentVariable[] {
  const text = document.getText();
  const variables: DocumentVariable[] = [];
  const varRegex = /^\s*(?:export\s+)?(let|const)\s+(\w+)\s*:\s*(\w+(?:\[\])?)/gm;
  let match;
  while ((match = varRegex.exec(text)) !== null) {
    variables.push({
      name: match[2],
      type: match[3],
      isConst: match[1] === 'const',
    });
  }
  return variables;
}

function extractFunctions(document: vscode.TextDocument): DocumentFunction[] {
  const text = document.getText();
  const functions: DocumentFunction[] = [];
  const funcRegex = /^\s*(?:export\s+)?function\s+(\w+)\s*\(([^)]*)\)\s*(?::\s*(\w+(?:\[\])?))?/gm;
  let match;
  while ((match = funcRegex.exec(text)) !== null) {
    functions.push({
      name: match[1],
      params: match[2].trim(),
      returnType: match[3] || 'void',
    });
  }
  return functions;
}

function getNamespaceBeforeDot(document: vscode.TextDocument, position: vscode.Position): string | undefined {
  const lineText = document.lineAt(position.line).text;
  const textBeforeCursor = lineText.substring(0, position.character);
  const match = textBeforeCursor.match(/(\w+)\.$/);
  return match ? match[1] : undefined;
}

function isSshConnectionVariable(word: string, document: vscode.TextDocument): boolean {
  const text = document.getText();
  const pattern = new RegExp(`\\b${word}\\b\\s*=\\s*ssh\\.connect\\b`);
  return pattern.test(text);
}

export function createCompletionProvider(): vscode.CompletionItemProvider {
  return {
    provideCompletionItems(
      document: vscode.TextDocument,
      position: vscode.Position,
      _token: vscode.CancellationToken,
      context: vscode.CompletionContext
    ): vscode.CompletionItem[] | undefined {
      const lineText = document.lineAt(position.line).text;
      const textBeforeCursor = lineText.substring(0, position.character);

      // Check if there's a dot context (either just typed '.' or cursor is after 'word.')
      const dotMatch = textBeforeCursor.match(/(\w+)\.\w*$/);

      if (dotMatch) {
        const namespaceName = dotMatch[1];

        // Check known namespaces
        const ns = namespaces[namespaceName];
        if (ns) {
          return ns.members.map((member) => {
            const item = new vscode.CompletionItem(member.label, member.kind);
            item.detail = member.detail;
            item.documentation = new vscode.MarkdownString(member.documentation);
            return item;
          });
        }

        // Check if it's an SSH connection variable
        if (isSshConnectionVariable(namespaceName, document)) {
          return sshConnectionMembers.map((member) => {
            const item = new vscode.CompletionItem(member.label, member.kind);
            item.detail = member.detail;
            item.documentation = new vscode.MarkdownString(member.documentation);
            return item;
          });
        }

        return [];
      }

      // Top-level completions: only when NOT triggered by dot
      if (context.triggerCharacter === '.') {
        return [];
      }

      const items: vscode.CompletionItem[] = [];

      // Keywords
      for (const kw of keywords) {
        const item = new vscode.CompletionItem(kw.label, vscode.CompletionItemKind.Keyword);
        item.detail = kw.detail;
        item.documentation = new vscode.MarkdownString(kw.documentation);
        items.push(item);
      }

      // Namespace modules
      for (const [name, ns] of Object.entries(namespaces)) {
        const item = new vscode.CompletionItem(name, vscode.CompletionItemKind.Module);
        item.detail = name;
        item.documentation = new vscode.MarkdownString(ns.description);
        items.push(item);
      }

      // User-defined variables
      for (const v of extractVariables(document)) {
        const item = new vscode.CompletionItem(v.name, v.isConst ? vscode.CompletionItemKind.Constant : vscode.CompletionItemKind.Variable);
        item.detail = `${v.isConst ? 'const' : 'let'} ${v.name}: ${v.type}`;
        item.documentation = new vscode.MarkdownString(`User-defined ${v.isConst ? 'constant' : 'variable'} of type \`${v.type}\``);
        items.push(item);
      }

      // User-defined functions
      for (const f of extractFunctions(document)) {
        const item = new vscode.CompletionItem(f.name, vscode.CompletionItemKind.Function);
        item.detail = `function ${f.name}(${f.params}): ${f.returnType}`;
        item.documentation = new vscode.MarkdownString(`User-defined function`);
        items.push(item);
      }

      return items;
    },
  };
}
