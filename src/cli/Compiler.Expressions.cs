using System.Text;
using System.Text.RegularExpressions;

public partial class Compiler
{
  private static int _randomCounter = 0;
  private static int _joinCounter = 0;
  private static int _sortCounter = 0;

  private string CompileExpression(Expression expr)
  {
    switch (expr)
    {
      case AssignmentExpression assign:
        // Handle array assignment specially
        if (assign.Left is ArrayAccess arrayAccess)
        {
          var arrayName = arrayAccess.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(arrayAccess.Array));
          var index = CompileExpression(arrayAccess.Index);
          var value = CompileExpression(assign.Right);

          // For array indices, we need to handle the compiled expression properly
          // If it's an array length expression, it will be quoted and we need to remove quotes
          if (index.StartsWith("\"") && index.EndsWith("\""))
          {
            index = index[1..^1]; // Remove quotes
          }

          return $"{arrayName}[{index}]={value}";
        }
        else
        {
          var varName = assign.Left is VariableExpression varExpr2 ? varExpr2.Name : CompileExpression(assign.Left);
          var value = CompileExpression(assign.Right);
          return $"{varName}={value}";
        }
      case LiteralExpression lit:
        return lit.Type == "string" ? $"\"{lit.Value}\"" : lit.Value;
      case VariableExpression var:
        return $"${{{var.Name}}}";
      case BinaryExpression bin:
        return CompileBinaryExpression(bin);
      case UnaryExpression un:
        return CompileUnaryExpression(un);
      case PostIncrementExpression postInc:
        return CompilePostIncrementExpression(postInc);
      case PostDecrementExpression postDec:
        return CompilePostDecrementExpression(postDec);
      case PreIncrementExpression preInc:
        return CompilePreIncrementExpression(preInc);
      case PreDecrementExpression preDec:
        return CompilePreDecrementExpression(preDec);
      case TernaryExpression tern:
        return CompileTernaryExpression(tern);
      case ParenthesizedExpression paren:
        return CompileParenthesizedExpression(paren);
      case StringInterpolationExpression stringInterpolation:
        return CompileStringInterpolationExpression(stringInterpolation);
      case ArrayLiteral arr:
        return CompileArrayLiteral(arr);
      case ArrayAccess acc:
        return CompileArrayAccess(acc);
      case ArrayLength len:
        return CompileArrayLength(len);
      case ArrayIsEmpty isEmpty:
        return CompileArrayIsEmpty(isEmpty);
      case ArrayContains contains:
        return CompileArrayContains(contains);
      case ArrayReverse reverse:
        return CompileArrayReverse(reverse);
      case FunctionCall func:
        return CompileFunctionCallExpression(func);
      case ConsoleIsSudoExpression sudo:
        return CompileConsoleIsSudoExpression(sudo);
      case ConsoleIsInteractiveExpression interactive:
        return CompileConsoleIsInteractiveExpression(interactive);
      case ConsoleGetShellExpression getShell:
        return CompileConsoleGetShellExpression(getShell);
      case ConsolePromptYesNoExpression prompt:
        return CompileConsolePromptYesNoExpression(prompt);
      case ConsoleShowMessageExpression showMessage:
        return CompileConsoleShowMessageExpression(showMessage);
      case ConsoleShowInfoExpression showInfo:
        return CompileConsoleShowInfoExpression(showInfo);
      case ConsoleShowWarningExpression showWarning:
        return CompileConsoleShowWarningExpression(showWarning);
      case ConsoleShowErrorExpression showError:
        return CompileConsoleShowErrorExpression(showError);
      case ConsoleShowSuccessExpression showSuccess:
        return CompileConsoleShowSuccessExpression(showSuccess);
      case ConsoleShowChoiceExpression showChoice:
        return CompileConsoleShowChoiceExpression(showChoice);
      case ConsoleShowMultiChoiceExpression showMultiChoice:
        return CompileConsoleShowMultiChoiceExpression(showMultiChoice);
      case ConsoleShowConfirmExpression showConfirm:
        return CompileConsoleShowConfirmExpression(showConfirm);
      case ConsoleShowProgressExpression showProgress:
        return CompileConsoleShowProgressExpression(showProgress);
      case ConsolePromptTextExpression promptText:
        return CompileConsolePromptTextExpression(promptText);
      case ConsolePromptPasswordExpression promptPassword:
        return CompileConsolePromptPasswordExpression(promptPassword);
      case ConsolePromptNumberExpression promptNumber:
        return CompileConsolePromptNumberExpression(promptNumber);
      case ConsolePromptFileExpression promptFile:
        return CompileConsolePromptFileExpression(promptFile);
      case ConsolePromptDirectoryExpression promptDirectory:
        return CompileConsolePromptDirectoryExpression(promptDirectory);
      case ArgsHasExpression argsHas:
        return CompileArgsHasExpression(argsHas);
      case ArgsGetExpression argsGet:
        return CompileArgsGetExpression(argsGet);
      case ArgsAllExpression argsAll:
        return CompileArgsAllExpression(argsAll);
      case OsIsInstalledExpression osInstalled:
        return CompileOsIsInstalledExpression(osInstalled);
      case ProcessElapsedTimeExpression elapsed:
        return CompileProcessElapsedTimeExpression(elapsed);
      case ProcessIdExpression processId:
        return CompileProcessIdExpression(processId);
      case ProcessCpuExpression processCpu:
        return CompileProcessCpuExpression(processCpu);
      case ProcessMemoryExpression processMemory:
        return CompileProcessMemoryExpression(processMemory);
      case ProcessCommandExpression processCommand:
        return CompileProcessCommandExpression(processCommand);
      case ProcessStatusExpression processStatus:
        return CompileProcessStatusExpression(processStatus);
      case TimerStopExpression:
        return CompileTimerStopExpression();
      case OsGetLinuxVersionExpression osLinuxVersion:
        return CompileOsGetLinuxVersionExpression(osLinuxVersion);
      case OsGetOSExpression osGetOS:
        return CompileOsGetOSExpression(osGetOS);
      case UtilityRandomExpression rand:
        return CompileUtilityRandomExpression(rand);
      case UtilityUuidExpression uuid:
        return CompileUtilityUuidExpression(uuid);
      case UtilityHashExpression hash:
        return CompileUtilityHashExpression(hash);
      case UtilityBase64EncodeExpression base64Encode:
        return CompileUtilityBase64EncodeExpression(base64Encode);
      case UtilityBase64DecodeExpression base64Decode:
        return CompileUtilityBase64DecodeExpression(base64Decode);
      case WebGetExpression webGet:
        return CompileWebGetExpression(webGet);
      case ArrayJoinExpression arrayJoin:
        return CompileArrayJoinExpression(arrayJoin);
      case ArraySortExpression arraySort:
        return CompileArraySortExpression(arraySort);
      case ArrayMergeExpression arrayMerge:
        return CompileArrayMergeExpression(arrayMerge);
      case ArrayShuffleExpression arrayShuffle:
        return CompileArrayShuffleExpression(arrayShuffle);
      case FsDirnameExpression fsDirname:
        return CompileFsDirnameExpression(fsDirname);
      case FsFileNameExpression fsFileName:
        return CompileFsFileNameExpression(fsFileName);
      case FsExtensionExpression fsExtension:
        return CompileFsExtensionExpression(fsExtension);
      case FsParentDirNameExpression fsParentDirName:
        return CompileFsParentDirNameExpression(fsParentDirName);
      case FsExistsExpression fsExists:
        return CompileFsExistsExpression(fsExists);
      case FsCreateTempFolderExpression fsCreateTempFolder:
        return CompileFsCreateTempFolderExpression(fsCreateTempFolder);
      case FsReadFileExpression fsReadFile:
        return CompileFsReadFileExpression(fsReadFile);
      case FsCopyExpression fsCopy:
        return CompileFsCopyExpression(fsCopy);
      case FsMoveExpression fsMove:
        return CompileFsMoveExpression(fsMove);
      case FsRenameExpression fsRename:
        return CompileFsRenameExpression(fsRename);
      case FsDeleteExpression fsDelete:
        return CompileFsDeleteExpression(fsDelete);
      case FsFindExpression fsFind:
        return CompileFsFindExpression(fsFind);
      case TimerCurrentExpression timerCurrent:
        return CompileTimerCurrentExpression(timerCurrent);
      case GitUndoLastCommitExpression:
        return CompileGitUndoLastCommitExpression();
      case JsonParseExpression jsonParse:
        return CompileJsonParseExpression(jsonParse);
      case JsonStringifyExpression jsonStringify:
        return CompileJsonStringifyExpression(jsonStringify);
      case JsonIsValidExpression jsonIsValid:
        return CompileJsonIsValidExpression(jsonIsValid);
      case JsonGetExpression jsonGet:
        return CompileJsonGetExpression(jsonGet);
      case JsonSetExpression jsonSet:
        return CompileJsonSetExpression(jsonSet);
      case JsonHasExpression jsonHas:
        return CompileJsonHasExpression(jsonHas);
      case JsonDeleteExpression jsonDelete:
        return CompileJsonDeleteExpression(jsonDelete);
      case JsonKeysExpression jsonKeys:
        return CompileJsonKeysExpression(jsonKeys);
      case JsonValuesExpression jsonValues:
        return CompileJsonValuesExpression(jsonValues);
      case JsonMergeExpression jsonMerge:
        return CompileJsonMergeExpression(jsonMerge);
      case JsonInstallDependenciesExpression jsonInstallDependencies:
        return CompileJsonInstallDependenciesExpression(jsonInstallDependencies);
      case YamlParseExpression yamlParse:
        return CompileYamlParseExpression(yamlParse);
      case YamlStringifyExpression yamlStringify:
        return CompileYamlStringifyExpression(yamlStringify);
      case YamlIsValidExpression yamlIsValid:
        return CompileYamlIsValidExpression(yamlIsValid);
      case YamlGetExpression yamlGet:
        return CompileYamlGetExpression(yamlGet);
      case YamlSetExpression yamlSet:
        return CompileYamlSetExpression(yamlSet);
      case YamlHasExpression yamlHas:
        return CompileYamlHasExpression(yamlHas);
      case YamlDeleteExpression yamlDelete:
        return CompileYamlDeleteExpression(yamlDelete);
      case YamlKeysExpression yamlKeys:
        return CompileYamlKeysExpression(yamlKeys);
      case YamlValuesExpression yamlValues:
        return CompileYamlValuesExpression(yamlValues);
      case YamlMergeExpression yamlMerge:
        return CompileYamlMergeExpression(yamlMerge);
      case YamlInstallDependenciesExpression yamlInstallDependencies:
        return CompileYamlInstallDependenciesExpression(yamlInstallDependencies);
      case SchedulerCronExpression schedulerCron:
        return CompileSchedulerCronExpression(schedulerCron);
      case LambdaExpression lambda:
        return CompileLambdaExpression(lambda);
      case FsWriteFileExpressionPlaceholder fsWriteFile:
        throw new InvalidOperationException("FsWriteFileExpressionPlaceholder should have been converted to a statement.");
      case TemplateUpdateExpression templateUpdate:
        return CompileTemplateUpdateExpression(templateUpdate);
      case StringNamespaceCallExpression stringNamespaceCall:
        return CompileStringNamespaceCallExpression(stringNamespaceCall);
      case ArrayNamespaceCallExpression arrayNamespaceCall:
        return CompileArrayNamespaceCallExpression(arrayNamespaceCall);
      default:
        throw new NotSupportedException($"Expression type {expr.GetType().Name} is not supported");
    }
  }

  private string CompileGitUndoLastCommitExpression()
  {
    // Generates bash code for git.undoLastCommit()
    return "$(git reset --soft HEAD~1)";
  }

  private string CompileConsoleIsSudoExpression(ConsoleIsSudoExpression sudo)
  {
    return "$([ \"$(id -u)\" -eq 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileConsoleIsInteractiveExpression(ConsoleIsInteractiveExpression interactive)
  {
    return "$([ -t 0 ] && echo \"true\" || echo \"false\")";
  }

  private string CompileConsoleGetShellExpression(ConsoleGetShellExpression getShell)
  {
    // Return the shell name by checking the SHELL environment variable and extracting the basename
    // Falls back to checking $0 if SHELL is not available
    return "$(basename \"${SHELL:-$0}\")";
  }

  private string CompileConsolePromptYesNoExpression(ConsolePromptYesNoExpression prompt)
  {
    // Compile the prompt text expression and extract the string value
    var promptText = CompileExpression(prompt.PromptText);
    // Remove quotes if it's a string literal
    if (promptText.StartsWith("\"") && promptText.EndsWith("\""))
    {
      promptText = promptText[1..^1];
    }

    // Automatically append " (y/n): " if not already present
    if (!promptText.EndsWith(" (y/n): ") && !promptText.EndsWith("(y/n): ") && !promptText.EndsWith(" (y/n):") && !promptText.EndsWith("(y/n):"))
    {
      promptText += " (y/n): ";
    }

    // Generate bash code that prompts the user and returns true/false based on yes/no response
    return $"$(while true; do read -p \"{promptText}\" yn; case $yn in [Yy]* ) echo \"true\"; break;; [Nn]* ) echo \"false\"; break;; * ) echo \"Please answer yes or no.\";; esac; done)";
  }

  private string CompileConsoleShowMessageExpression(ConsoleShowMessageExpression showMessage)
  {
    var title = CompileExpression(showMessage.Title);
    var message = CompileExpression(showMessage.Message);

    // Use a more robust approach that avoids very long single lines
    return $"$({{ if command -v dialog >/dev/null 2>&1; then dialog --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; else echo {title}: {message}; read -p \"Press Enter...\"; fi; }} 2>/dev/null; echo \"\")";
  }
  private string CompileConsoleShowInfoExpression(ConsoleShowInfoExpression showInfo)
  {
    var title = CompileExpression(showInfo.Title);
    var message = CompileExpression(showInfo.Message);
    return $"$(if command -v dialog >/dev/null 2>&1; then dialog --title {title} --infobox {message} 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title {title} --infobox {message} 10 60 2>&1 >/dev/tty; else echo \"[INFO] \" {title} \": \" {message}; fi; echo \"\")";
  }

  private string CompileConsoleShowWarningExpression(ConsoleShowWarningExpression showWarning)
  {
    var title = CompileExpression(showWarning.Title);
    var message = CompileExpression(showWarning.Message);
    return $"$(if command -v dialog >/dev/null 2>&1; then dialog --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; else echo \"[WARNING] \" {title} \": \" {message}; read -p \"Press Enter to continue...\"; fi; echo \"\")";
  }

  private string CompileConsoleShowErrorExpression(ConsoleShowErrorExpression showError)
  {
    var title = CompileExpression(showError.Title);
    var message = CompileExpression(showError.Message);
    return $"$(if command -v dialog >/dev/null 2>&1; then dialog --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; else echo \"[ERROR] \" {title} \": \" {message} >&2; read -p \"Press Enter to continue...\"; fi; echo \"\")";
  }

  private string CompileConsoleShowSuccessExpression(ConsoleShowSuccessExpression showSuccess)
  {
    var title = CompileExpression(showSuccess.Title);
    var message = CompileExpression(showSuccess.Message);
    return $"$(if command -v dialog >/dev/null 2>&1; then dialog --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; elif command -v whiptail >/dev/null 2>&1; then whiptail --title {title} --msgbox {message} 10 60 2>&1 >/dev/tty; else echo \"[SUCCESS] \" {title} \": \" {message}; read -p \"Press Enter to continue...\"; fi; echo \"\")";
  }

  private string CompileConsoleShowChoiceExpression(ConsoleShowChoiceExpression showChoice)
  {
    var title = CompileExpression(showChoice.Title);
    var message = CompileExpression(showChoice.Message);
    var options = CompileExpression(showChoice.Options);
    var defaultIndex = showChoice.DefaultIndex != null ? CompileExpression(showChoice.DefaultIndex) : "0";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"_utah_choice_result=$(echo {options} | tr ',' '\\n' | nl -n ln | dialog --title {title} --default-item {defaultIndex} --menu {message} 15 60 10 --file - 2>&1 >/dev/tty); " +
           $"echo {options} | cut -d',' -f$_utah_choice_result; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"_utah_choice_result=$(echo {options} | tr ',' '\\n' | nl -n ln | whiptail --title {title} --default-item {defaultIndex} --menu {message} 15 60 10 --file - 2>&1 >/dev/tty); " +
           $"echo {options} | cut -d',' -f$_utah_choice_result; " +
           $"else echo \"Choose from: \" {options}; read -p \"Enter your choice: \" _utah_fallback_choice; echo $_utah_fallback_choice; fi)";
  }

  private string CompileConsoleShowMultiChoiceExpression(ConsoleShowMultiChoiceExpression showMultiChoice)
  {
    var title = CompileExpression(showMultiChoice.Title);
    var message = CompileExpression(showMultiChoice.Message);
    var options = CompileExpression(showMultiChoice.Options);
    var defaultSelected = showMultiChoice.DefaultSelected != null ? CompileExpression(showMultiChoice.DefaultSelected) : "\"\"";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"echo {options} | tr ',' '\\n' | nl -n ln | dialog --title {title} --checklist {message} 15 60 10 --file - 2>&1 >/dev/tty | tr '\\n' ','; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"echo {options} | tr ',' '\\n' | nl -n ln | whiptail --title {title} --checklist {message} 15 60 10 --file - 2>&1 >/dev/tty | tr '\\n' ','; " +
           $"else echo \"Select from: \" {options}; read -p \"Enter your selections (comma-separated): \" _utah_multi_choice; echo $_utah_multi_choice; fi)";
  }

  private string CompileConsoleShowConfirmExpression(ConsoleShowConfirmExpression showConfirm)
  {
    var title = CompileExpression(showConfirm.Title);
    var message = CompileExpression(showConfirm.Message);
    var defaultButton = showConfirm.DefaultButton != null ? CompileExpression(showConfirm.DefaultButton) : "\"yes\"";

    var defaultYes = $"[[ {defaultButton} == \"yes\" ]]";
    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"if dialog --title {title} --yesno {message} 10 60 2>&1 >/dev/tty; then echo \"true\"; else echo \"false\"; fi; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"if whiptail --title {title} --yesno {message} 10 60 2>&1 >/dev/tty; then echo \"true\"; else echo \"false\"; fi; " +
           $"else while true; do if {defaultYes}; then read -p {message} \" (Y/n): \" _utah_confirm; else read -p {message} \" (y/N): \" _utah_confirm; fi; " +
           $"case $_utah_confirm in [Yy]*|'') echo \"true\"; break;; [Nn]*) echo \"false\"; break;; *) echo \"Please answer yes or no.\";; esac; done; fi)";
  }

  private string CompileConsoleShowProgressExpression(ConsoleShowProgressExpression showProgress)
  {
    var title = CompileExpression(showProgress.Title);
    var message = CompileExpression(showProgress.Message);
    var percent = CompileExpression(showProgress.Percent);
    var canCancel = showProgress.CanCancel != null ? CompileExpression(showProgress.CanCancel) : "false";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"echo {percent} | dialog --title {title} --gauge {message} 10 60 0 2>&1 >/dev/tty; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"echo {percent} | whiptail --title {title} --gauge {message} 10 60 0 2>&1 >/dev/tty; " +
           $"else echo \"[\" {percent} \"%] \" {title} \": \" {message}; fi; echo \"\")";
  }

  private string CompileConsolePromptTextExpression(ConsolePromptTextExpression promptText)
  {
    var prompt = CompileExpression(promptText.Prompt);
    var defaultValue = promptText.DefaultValue != null ? CompileExpression(promptText.DefaultValue) : "\"\"";
    var validation = promptText.ValidationPattern != null ? CompileExpression(promptText.ValidationPattern) : "\"\"";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"dialog --title \"Input\" --inputbox {prompt} 10 60 {defaultValue} 2>&1 >/dev/tty; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"whiptail --title \"Input\" --inputbox {prompt} 10 60 {defaultValue} 2>&1 >/dev/tty; " +
           $"else if [[ -n {defaultValue} ]]; then read -p {prompt} \" [\" {defaultValue} \"]: \" _utah_input; echo ${{_utah_input:-{defaultValue}}}; else read -p {prompt} \": \" _utah_input; echo $_utah_input; fi; fi)";
  }

  private string CompileConsolePromptPasswordExpression(ConsolePromptPasswordExpression promptPassword)
  {
    var prompt = CompileExpression(promptPassword.Prompt);
    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"dialog --title \"Password\" --passwordbox {prompt} 10 60 2>&1 >/dev/tty; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"whiptail --title \"Password\" --passwordbox {prompt} 10 60 2>&1 >/dev/tty; " +
           $"else read -s -p {prompt} \": \" _utah_password; echo $_utah_password; fi)";
  }

  private string CompileConsolePromptNumberExpression(ConsolePromptNumberExpression promptNumber)
  {
    var prompt = CompileExpression(promptNumber.Prompt);
    var minValue = promptNumber.MinValue != null ? CompileExpression(promptNumber.MinValue) : "\"\"";
    var maxValue = promptNumber.MaxValue != null ? CompileExpression(promptNumber.MaxValue) : "\"\"";
    var defaultValue = promptNumber.DefaultValue != null ? CompileExpression(promptNumber.DefaultValue) : "\"\"";

    return $"$(while true; do " +
           $"if [[ -n {defaultValue} ]]; then read -p {prompt} \" [\" {defaultValue} \"]: \" _utah_number; _utah_number=${{_utah_number:-{defaultValue}}}; else read -p {prompt} \": \" _utah_number; fi; " +
           $"if [[ $_utah_number =~ ^-?[0-9]+$ ]]; then " +
           $"_utah_valid=true; " +
           $"if [[ -n {minValue} && $_utah_number -lt {minValue} ]]; then echo \"Value must be >= \" {minValue}; _utah_valid=false; fi; " +
           $"if [[ -n {maxValue} && $_utah_number -gt {maxValue} ]]; then echo \"Value must be <= \" {maxValue}; _utah_valid=false; fi; " +
           $"if [[ $_utah_valid == true ]]; then echo $_utah_number; break; fi; " +
           $"else echo \"Please enter a valid number.\"; fi; done)";
  }

  private string CompileConsolePromptFileExpression(ConsolePromptFileExpression promptFile)
  {
    var prompt = CompileExpression(promptFile.Prompt);
    var filter = promptFile.Filter != null ? CompileExpression(promptFile.Filter) : "\"*\"";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"dialog --title \"File Selection\" --fselect \"$(pwd)/\" 15 80 2>&1 >/dev/tty; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"whiptail --title \"File Selection\" --inputbox {prompt} \" (current dir: $(pwd))\" 10 60 2>&1 >/dev/tty; " +
           $"else read -p {prompt} \": \" _utah_file; echo $_utah_file; fi)";
  }

  private string CompileConsolePromptDirectoryExpression(ConsolePromptDirectoryExpression promptDirectory)
  {
    var prompt = CompileExpression(promptDirectory.Prompt);
    var defaultPath = promptDirectory.DefaultPath != null ? CompileExpression(promptDirectory.DefaultPath) : "\"$(pwd)\"";

    return $"$(if command -v dialog >/dev/null 2>&1; then " +
           $"dialog --title \"Directory Selection\" --dselect {defaultPath} 15 80 2>&1 >/dev/tty; " +
           $"elif command -v whiptail >/dev/null 2>&1; then " +
           $"whiptail --title \"Directory Selection\" --inputbox {prompt} \" [\" {defaultPath} \"]:\" 10 60 {defaultPath} 2>&1 >/dev/tty; " +
           $"else if [[ -n {defaultPath} ]]; then read -p {prompt} \" [\" {defaultPath} \"]: \" _utah_dir; echo ${{_utah_dir:-{defaultPath}}}; else read -p {prompt} \": \" _utah_dir; echo $_utah_dir; fi; fi)";
  }

  private string CompileOsIsInstalledExpression(OsIsInstalledExpression osInstalled)
  {
    // Generate bash code that checks if a command exists and returns true/false
    string appReference;

    if (osInstalled.AppName is VariableExpression varExpr)
    {
      appReference = $"${{{varExpr.Name}}}";
    }
    else if (osInstalled.AppName is LiteralExpression literal && literal.Type == "string")
    {
      appReference = $"\"{literal.Value}\"";
    }
    else
    {
      appReference = CompileExpression(osInstalled.AppName);
    }

    return $"$(command -v {appReference} &> /dev/null && echo \"true\" || echo \"false\")";
  }

  private string CompileProcessElapsedTimeExpression(ProcessElapsedTimeExpression elapsed)
  {
    // Generate bash code that gets the elapsed time since the process started
    // Using ps command to get the elapsed time of the current process
    return "$(ps -o etime -p $$ --no-headers | tr -d ' ')";
  }

  private string CompileProcessIdExpression(ProcessIdExpression processId)
  {
    return "$(ps -o pid -p $$ --no-headers | tr -d ' ')";
  }

  private string CompileProcessCpuExpression(ProcessCpuExpression processCpu)
  {
    return "$(ps -o pcpu -p $$ --no-headers | tr -d ' ' | awk '{printf(\"%d\", $1 + 0.5)}')";
  }

  private string CompileProcessMemoryExpression(ProcessMemoryExpression processMemory)
  {
    return "$(ps -o pmem -p $$ --no-headers | tr -d ' ' | awk '{printf(\"%d\", $1 + 0.5)}')";
  }

  private string CompileProcessCommandExpression(ProcessCommandExpression processCommand)
  {
    return "$(ps -o cmd= -p $$)";
  }

  private string CompileProcessStatusExpression(ProcessStatusExpression processStatus)
  {
    return "$(ps -o stat= -p $$)";
  }

  private string CompileOsGetLinuxVersionExpression(OsGetLinuxVersionExpression osLinuxVersion)
  {
    // Generate the complex bash script for getting Linux version
    return "$(" +
           "if [[ -f /etc/os-release ]]; then " +
           "source /etc/os-release; " +
           "echo \"${VERSION_ID}\"; " +
           "elif type lsb_release >/dev/null 2>&1; then " +
           "lsb_release -sr; " +
           "elif [[ -f /etc/lsb-release ]]; then " +
           "source /etc/lsb-release; " +
           "echo \"${DISTRIB_RELEASE}\"; " +
           "else " +
           "echo \"unknown\"; " +
           "fi)";
  }

  private string CompileOsGetOSExpression(OsGetOSExpression osGetOS)
  {
    // Generate the complex bash script for getting OS type
    return "$(" +
           "_uname_os_get_os=$(uname | tr '[:upper:]' '[:lower:]'); " +
           "case $_uname_os_get_os in " +
           "linux*) echo \"linux\" ;; " +
           "darwin*) echo \"mac\" ;; " +
           "msys*|cygwin*|mingw*|nt|win*) echo \"windows\" ;; " +
           "*) echo \"unknown\" ;; " +
           "esac)";
  }

  private string CompileUtilityRandomExpression(UtilityRandomExpression rand)
  {
    // Generate unique variable names to avoid conflicts
    _randomCounter++;
    var minVar = $"_utah_random_min_{_randomCounter}";
    var maxVar = $"_utah_random_max_{_randomCounter}";

    var minValue = "0";
    var maxValue = "32767";

    // Handle parameters
    if (rand.MinValue != null && rand.MaxValue != null)
    {
      // Both min and max provided: utility.random(min, max)
      minValue = CompileExpression(rand.MinValue);
      maxValue = CompileExpression(rand.MaxValue);
    }
    else if (rand.MaxValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MaxValue);
    }
    // If no parameters, use defaults (0, 32767)

    // Remove quotes from numeric values if present
    minValue = minValue.Trim('"');
    maxValue = maxValue.Trim('"');

    // For variable references, we need to use the actual variable values
    // If the value starts with ${, it's a variable reference
    if (minValue.StartsWith("${") && minValue.EndsWith("}"))
    {
      // Already in correct format ${varName}
    }
    else if (minValue.StartsWith("$((") && minValue.EndsWith("))"))
    {
      // It's an arithmetic expression like $((expr)), use it directly
    }
    else if (!int.TryParse(minValue, out _))
    {
      // It's a variable name without ${}, add $ prefix
      minValue = $"${minValue}";
    }

    if (maxValue.StartsWith("${") && maxValue.EndsWith("}"))
    {
      // It's a variable, use it directly
    }
    else if (maxValue.StartsWith("$((") && maxValue.EndsWith("))"))
    {
      // It's an arithmetic expression like $((expr)), use it directly
    }
    else if (!int.TryParse(maxValue, out _))
    {
      // It's not a valid integer, maybe it's a variable name without ${}
      maxValue = $"${{{maxValue}}}";
    }

    // Return the bash command substitution that generates the random number with validation
    return $"$({minVar}={minValue}; {maxVar}={maxValue}; if [ ${minVar} -gt ${maxVar} ]; then echo \"Error: min value (${minVar}) cannot be greater than max value (${maxVar}) in utility.random()\" >&2; exit 100; fi; echo $((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar})))";
  }

  private List<string> CompileUtilityRandomDeclaration(string variableName, UtilityRandomExpression rand, bool isConst)
  {
    var lines = new List<string>();

    // Generate unique variable names to avoid conflicts
    _randomCounter++;
    var minVar = $"_utah_random_min_{_randomCounter}";
    var maxVar = $"_utah_random_max_{_randomCounter}";

    var minValue = "0";
    var maxValue = "32767";

    // Handle parameters
    if (rand.MinValue != null && rand.MaxValue != null)
    {
      // Both min and max provided: utility.random(min, max)
      minValue = CompileExpression(rand.MinValue);
      maxValue = CompileExpression(rand.MaxValue);
    }
    else if (rand.MaxValue != null)
    {
      // Only one parameter provided: utility.random(max)
      minValue = "0";
      maxValue = CompileExpression(rand.MaxValue);
    }
    // If no parameters, use defaults (0, 32767)

    // Remove quotes from numeric values if present
    minValue = minValue.Trim('"');
    maxValue = maxValue.Trim('"');

    // For variable references, we need to use the actual variable values
    if (minValue.StartsWith("${") && minValue.EndsWith("}"))
    {
      // Already in correct format ${varName}
    }
    else if (!int.TryParse(minValue, out _))
    {
      // It's a variable name without ${}, add $ prefix
      minValue = $"${minValue}";
    }

    if (maxValue.StartsWith("${") && maxValue.EndsWith("}"))
    {
      // It's a variable, use it directly
    }
    else if (!int.TryParse(maxValue, out _))
    {
      // It's not a valid integer, maybe it's a variable name without ${}
      maxValue = $"${{{maxValue}}}";
    }

    // Generate validation and assignment as separate statements
    lines.Add($"{minVar}={minValue}");
    lines.Add($"{maxVar}={maxValue}");
    lines.Add($"if [ ${minVar} -gt ${maxVar} ]; then");
    lines.Add($"  echo \"Error: min value (${minVar}) cannot be greater than max value (${maxVar}) in utility.random()\" >&2");
    lines.Add($"  exit 100");
    lines.Add($"fi");

    // Generate the assignment
    if (isConst)
    {
      lines.Add($"readonly {variableName}=$((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar}))");
    }
    else
    {
      lines.Add($"{variableName}=$((RANDOM * ({maxVar} - {minVar} + 1) / 32768 + {minVar}))");
    }

    return lines;
  }

  private string CompileUtilityUuidExpression(UtilityUuidExpression uuid)
  {
    // Generate UUID using uuidgen or fallback to other methods
    return "$(if command -v uuidgen >/dev/null 2>&1; then uuidgen; elif command -v python3 >/dev/null 2>&1; then python3 -c \"import uuid; print(uuid.uuid4())\"; else echo \"$(date +%s)-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))-$(($RANDOM * $RANDOM))\"; fi)";
  }

  private string CompileUtilityHashExpression(UtilityHashExpression hash)
  {
    var text = CompileExpression(hash.Text);
    var algorithm = hash.Algorithm != null ? CompileExpression(hash.Algorithm) : "\"sha256\"";

    return $"$(echo -n {text} | case {algorithm} in \"md5\") md5sum | cut -d' ' -f1 ;; \"sha1\") sha1sum | cut -d' ' -f1 ;; \"sha256\") sha256sum | cut -d' ' -f1 ;; \"sha512\") sha512sum | cut -d' ' -f1 ;; *) echo \"Error: Unsupported hash algorithm: {algorithm}\" >&2; exit 1 ;; esac)";
  }

  private string CompileUtilityBase64EncodeExpression(UtilityBase64EncodeExpression base64Encode)
  {
    var text = CompileExpression(base64Encode.Text);
    return $"$(echo -n {text} | base64 -w 0)";
  }

  private string CompileUtilityBase64DecodeExpression(UtilityBase64DecodeExpression base64Decode)
  {
    var text = CompileExpression(base64Decode.Text);
    return $"$(echo -n {text} | base64 -d)";
  }

  private string CompileLiteralExpression(LiteralExpression lit)
  {
    // For string literals, ensure they are quoted
    return lit.Type switch
    {
      "string" => $"\"{lit.Value}\"",
      "number" => lit.Value,
      "boolean" => lit.Value,
      _ => $"\"{lit.Value}\""
    };
  }

  private string CompileVariableExpression(VariableExpression var)
  {
    return $"${{{var.Name}}}";
  }

  private string CompileBinaryExpression(BinaryExpression bin)
  {
    var left = CompileExpression(bin.Left);
    var right = CompileExpression(bin.Right);

    return bin.Operator switch
    {
      "+" when IsStringConcatenation(bin) => CompileStringConcatenation(left, right),
      "+" => $"$(({GetArithmeticValue(left)} + {GetArithmeticValue(right)}))",
      "-" => $"$(({GetArithmeticValue(left)} - {GetArithmeticValue(right)}))",
      "*" => $"$(({GetArithmeticValue(left)} * {GetArithmeticValue(right)}))",
      "/" => $"$(({GetArithmeticValue(left)} / {GetArithmeticValue(right)}))",
      "%" => $"$(({GetArithmeticValue(left)} % {GetArithmeticValue(right)}))",
      "==" => IsNumericLiteral(bin.Right) ? $"[ {left} -eq {right} ]" : $"[ {EnsureQuoted(left)} = {EnsureQuoted(right)} ]",
      "===" => IsNumericLiteral(bin.Right) ? $"[ {left} -eq {right} ]" : $"[ {EnsureQuoted(left)} = {EnsureQuoted(right)} ]",
      "!=" => $"[ {EnsureQuoted(left)} != {EnsureQuoted(right)} ]",
      "<" => $"[ {left} -lt {right} ]",
      "<=" => $"[ {left} -le {right} ]",
      ">" => $"[ {left} -gt {right} ]",
      ">=" => $"[ {left} -ge {right} ]",
      "&&" => $"{left} && {right}",
      "||" => $"{left} || {right}",
      _ => throw new NotSupportedException($"Binary operator {bin.Operator} is not supported")
    };
  }

  private string CompileUnaryExpression(UnaryExpression un)
  {
    var operand = CompileExpression(un.Operand);
    return un.Operator switch
    {
      "!" => $"! {operand}",
      "-" => $"$((-${operand}))",
      _ => throw new NotSupportedException($"Unary operator {un.Operator} is not supported")
    };
  }

  private string CompilePostIncrementExpression(PostIncrementExpression postInc)
  {
    if (postInc.Operand is VariableExpression varExpr)
    {
      // For post-increment, we need to return the old value and increment the variable
      // In bash, this is complex, so we'll just return the variable and rely on
      // the statement context to handle the increment
      return $"${{{varExpr.Name}}}";
    }
    throw new NotSupportedException("Post-increment can only be applied to variables");
  }

  private string CompilePostDecrementExpression(PostDecrementExpression postDec)
  {
    if (postDec.Operand is VariableExpression varExpr)
    {
      return $"${{{varExpr.Name}}}";
    }
    throw new NotSupportedException("Post-decrement can only be applied to variables");
  }

  private string CompilePreIncrementExpression(PreIncrementExpression preInc)
  {
    if (preInc.Operand is VariableExpression varExpr)
    {
      // For pre-increment, we increment first then return the new value
      return $"$(({varExpr.Name} + 1))";
    }
    throw new NotSupportedException("Pre-increment can only be applied to variables");
  }

  private string CompilePreDecrementExpression(PreDecrementExpression preDec)
  {
    if (preDec.Operand is VariableExpression varExpr)
    {
      return $"$(({varExpr.Name} - 1))";
    }
    throw new NotSupportedException("Pre-decrement can only be applied to variables");
  }

  private string CompileTernaryExpression(TernaryExpression tern)
  {
    // Helper to compile the condition part of a ternary
    string CompileTernaryCondition(Expression expr)
    {
      if (expr is FunctionCall fc && fc.Name == "env.get" && fc.Arguments.Count == 1)
      {
        var varName = CompileExpression(fc.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        return $"[ -n \"${{{varName}}}\" ]";
      }
      if (expr is VariableExpression varExpr)
      {
        return $"[ \"${{{varExpr.Name}}}\" = \"true\" ]";
      }

      // For other expressions, compile them and if they don't start with '[', wrap them
      var compiledExpr = CompileExpression(expr);

      if (compiledExpr.StartsWith("[ ") && compiledExpr.EndsWith(" ]"))
      {
        return compiledExpr;
      }
      else
      {
        // Assume it's a truthy check
        return $"[ -n {compiledExpr} ]";
      }
    }

    // Helper to compile the value part of a ternary
    string CompileTernaryValue(Expression expr)
    {
      if (expr is FunctionCall fc && fc.Name == "env.get" && fc.Arguments.Count == 1)
      {
        var varName = CompileExpression(fc.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        return $"\"${{{varName}}}\"";
      }
      return CompileExpression(expr);
    }

    var condition = CompileTernaryCondition(tern.Condition);
    var trueExpr = CompileTernaryValue(tern.TrueExpression);

    // Handle nested ternary by avoiding nested command substitution
    if (tern.FalseExpression is TernaryExpression nestedTern)
    {
      // Recursively build the chain, removing the outer `$()` from the nested compilation
      var nestedTernaryStr = CompileTernaryExpression(nestedTern);
      if (nestedTernaryStr.StartsWith("$(") && nestedTernaryStr.EndsWith(")"))
      {
        var nestedChain = nestedTernaryStr.Substring(2, nestedTernaryStr.Length - 3);
        return $"$({condition} && echo {trueExpr} || {nestedChain})";
      }
      else
      {
        return $"$({condition} && echo {trueExpr} || {nestedTernaryStr})";
      }
    }

    var falseExpr = CompileTernaryValue(tern.FalseExpression);

    return $"$({condition} && echo {trueExpr} || echo {falseExpr})";
  }

  private string CompileParenthesizedExpression(ParenthesizedExpression paren)
  {
    var inner = CompileExpression(paren.Inner);

    // If the inner expression is already an arithmetic expression (starts with $((, don't add extra parentheses
    if (inner.StartsWith("$((") && inner.EndsWith("))"))
    {
      return inner;
    }

    return $"({inner})";
  }

  private string CompileStringConcatenation(string left, string right)
  {
    // Remove quotes from string literals
    var leftPart = left.StartsWith("\"") && left.EndsWith("\"") ? left[1..^1] : left;

    // Handle variable expressions - extract and format properly
    string rightPart;
    if (right.StartsWith("$(") && right.EndsWith(")"))
    {
      // Command substitution, use as is
      rightPart = right;
    }
    else if (right.StartsWith("${") && right.EndsWith("}"))
    {
      // Already in ${var} format, use as is
      rightPart = right;
    }
    else if (right.StartsWith("$"))
    {
      // Old $var format, convert to ${var}
      var varName = right.TrimStart('$');
      rightPart = $"${{{varName}}}";
    }
    else if (right.StartsWith("\"") && right.EndsWith("\""))
    {
      rightPart = right[1..^1];
    }
    else
    {
      rightPart = right;
    }

    return $"\"{leftPart}{rightPart}\"";
  }

  private bool IsStringConcatenation(BinaryExpression bin)
  {
    // Simple heuristic: if either operand is a string literal or we're adding strings
    return (bin.Left is LiteralExpression leftLit && leftLit.Type == "string") ||
           (bin.Right is LiteralExpression rightLit && rightLit.Type == "string");
  }

  private string CompileArrayLiteral(ArrayLiteral arr)
  {
    // Bash arrays are declared and initialized like: arr=("item1" "item2" "item3")
    var elements = arr.Elements.Select(CompileExpression).ToList();
    return $"({string.Join(" ", elements)})";
  }

  private string CompileArrayAccess(ArrayAccess acc)
  {
    // Get the array name directly if it's a variable expression
    var arrayName = acc.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(acc.Array));
    var index = CompileExpression(acc.Index);

    // In bash, array access is ${arrayName[index]}
    return $"${{{arrayName}[{ExtractVariableName(index)}]}}";
  }

  private string CompileArrayLength(ArrayLength len)
  {
    var arrayName = len.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(len.Array));

    // In bash, array length is ${#arrayName[@]}
    // Don't quote it as it's often used in arithmetic contexts
    return $"${{#{arrayName}[@]}}";
  }

  private string CompileArrayIsEmpty(ArrayIsEmpty isEmpty)
  {
    // Check if this is a simple variable or a complex expression
    if (isEmpty.Array is VariableExpression varExpr)
    {
      // Simple variable reference
      var arrayName = varExpr.Name;
      return $"$([ ${{#{arrayName}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
    }
    else
    {
      // Complex expression - we need to handle the result as an array
      var compiledArray = CompileExpression(isEmpty.Array);
      var uniqueVar = $"_utah_isempty_{GetUniqueId()}";

      // For command substitutions, we need to capture the output as an array
      if (compiledArray.StartsWith("$("))
      {
        return $"$({uniqueVar}=({compiledArray}); [ ${{#{uniqueVar}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
      }
      else
      {
        // For array literals, assign directly
        return $"$({uniqueVar}={compiledArray}; [ ${{#{uniqueVar}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
      }
    }
  }

  private string CompileArrayContains(ArrayContains contains)
  {
    var arrayName = contains.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(contains.Array));
    var item = CompileExpression(contains.Item);

    // Remove quotes from item if it's a string literal for comparison
    var itemValue = item.StartsWith("\"") && item.EndsWith("\"") ? item[1..^1] : item;

    // Use a simpler approach with case statement for better compatibility
    return $"$(case \" ${{{arrayName}[@]}} \" in *\" {itemValue} \"*) echo \"true\" ;; *) echo \"false\" ;; esac)";
  }

  private string CompileArrayReverse(ArrayReverse reverse)
  {
    var arrayName = reverse.Array is VariableExpression varExpr ? varExpr.Name : ExtractVariableName(CompileExpression(reverse.Array));

    // In bash, reverse an array by creating a new array with elements in reverse order
    // We use readarray to convert the output into an array
    return $"($(for ((i=${{#{arrayName}[@]}}-1; i>=0; i--)); do echo \"${{{arrayName}[i]}}\"; done))";
  }

  private string CompileFunctionCallExpression(FunctionCall func)
  {
    // Special handling for env.get() function calls
    if (func.Name == "env.get")
    {
      if (func.Arguments.Count == 1)
      {
        // env.get("VAR") -> "${VAR}"
        var varName = CompileExpression(func.Arguments[0]);
        // Remove quotes if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        return $"\"${{{varName}}}\"";
      }
      else if (func.Arguments.Count == 2)
      {
        // env.get("VAR", "default") -> "${VAR:-default}"
        var varName = CompileExpression(func.Arguments[0]);
        var defaultValue = CompileExpression(func.Arguments[1]);
        // Remove quotes from varName if it's a string literal
        if (varName.StartsWith("\"") && varName.EndsWith("\""))
        {
          varName = varName[1..^1];
        }
        // Remove quotes from defaultValue if it's a string literal
        if (defaultValue.StartsWith("\"") && defaultValue.EndsWith("\""))
        {
          defaultValue = defaultValue[1..^1];
        }
        return $"\"${{{varName}:-{defaultValue}}}\"";
      }
    }

    // In bash, function calls in expressions use command substitution
    var bashArgs = func.Arguments.Select(arg =>
    {
      var compiled = CompileExpression(arg);
      // Quote variable references for proper word splitting
      if (arg is VariableExpression && compiled.StartsWith("${") && compiled.EndsWith("}"))
      {
        return $"\"{compiled}\"";
      }
      return compiled;
    }).ToList();

    if (bashArgs.Count > 0)
    {
      return $"$({func.Name} {string.Join(" ", bashArgs)})";
    }
    else
    {
      return $"$({func.Name})";
    }
  }

  private string ExtractVariableName(string varExpr)
  {
    // Handle quoted strings - remove quotes
    if (varExpr.StartsWith("\"") && varExpr.EndsWith("\""))
    {
      varExpr = varExpr[1..^1]; // Remove quotes
    }

    // Handle array length expressions - keep them as-is for arithmetic
    if (varExpr.StartsWith("${#") && varExpr.EndsWith("[@]}"))
    {
      // This is an array length expression like ${#arrayName[@]}
      // In arithmetic contexts, return as-is
      return varExpr;
    }

    // Handle both $var and ${var} formats
    if (varExpr.StartsWith("${") && varExpr.EndsWith("}"))
    {
      return varExpr[2..^1]; // Remove ${ and }
    }
    else if (varExpr.StartsWith("$"))
    {
      return varExpr[1..]; // Remove $
    }

    // Handle complex expressions that are already unquoted
    if (varExpr.Contains("#") && varExpr.Contains("[@]"))
    {
      // This is an array length expression like #{arrayName[@]}
      return varExpr;
    }

    return varExpr;
  }

  private string GetArithmeticValue(string expression)
  {
    // Handle array length expressions - they should remain as-is in arithmetic contexts
    if (expression.StartsWith("${#") && expression.EndsWith("[@]}"))
    {
      return expression;
    }

    // Handle numeric literals - they should remain as-is
    if (int.TryParse(expression, out _) || double.TryParse(expression, out _))
    {
      return expression;
    }

    // Handle quoted numeric literals
    if (expression.StartsWith("\"") && expression.EndsWith("\""))
    {
      var innerValue = expression[1..^1];
      if (int.TryParse(innerValue, out _) || double.TryParse(innerValue, out _))
      {
        return innerValue;
      }
    }

    // For everything else, extract the variable name
    return ExtractVariableName(expression);
  }

  private bool IsNumericLiteral(Expression expr)
  {
    if (expr is LiteralExpression literal)
    {
      return literal.Type == "number" || int.TryParse(literal.Value, out _) || double.TryParse(literal.Value, out _);
    }
    return false;
  }

  private string CompileWebGetExpression(WebGetExpression webGet)
  {
    var url = CompileExpression(webGet.Url);

    // Handle different types of URL expressions
    string curlUrl;
    if (url.StartsWith("${") && url.EndsWith("}"))
    {
      // It's a variable reference like ${varName}
      curlUrl = url;
    }
    else if (url.StartsWith("\"") && url.EndsWith("\""))
    {
      // It's a string literal like "http://example.com"
      curlUrl = url; // Keep the quotes for curl
    }
    else
    {
      // It's a variable name without ${}, add $ prefix for bash
      curlUrl = $"${{{url}}}";
    }

    // Return a bash command substitution that uses curl to make the GET request
    return $"$(curl -s {curlUrl} 2>/dev/null || echo \"\")";
  }

  private string CompileArrayJoinExpression(ArrayJoinExpression arrayJoin)
  {
    var arrayName = ExtractVariableName(CompileExpression(arrayJoin.Array));
    var separator = CompileExpression(arrayJoin.Separator);

    _joinCounter++;
    var uniqueVar = $"_utah_join_{_joinCounter}";
    var separatorVar = $"_utah_sep_{_joinCounter}";

    return $"$({separatorVar}={separator}; {uniqueVar}=\"\"; for item in \"${{{arrayName}[@]}}\"; do if [ -n \"${{{uniqueVar}}}\" ]; then {uniqueVar}+=\"${{{separatorVar}}}\"; fi; {uniqueVar}+=\"$item\"; done; echo \"${{{uniqueVar}}}\")";
  }

  private string CompileArraySortExpression(ArraySortExpression arraySort)
  {
    var compiledArray = CompileExpression(arraySort.Array);

    _sortCounter++;
    var uniqueVar = $"_utah_sort_{_sortCounter}";

    // Determine sort order - default to ascending
    var sortOrder = "asc";
    if (arraySort.SortOrder != null)
    {
      var sortOrderValue = CompileExpression(arraySort.SortOrder);
      // Remove quotes if present
      if (sortOrderValue.StartsWith("\"") && sortOrderValue.EndsWith("\""))
      {
        sortOrder = sortOrderValue.Substring(1, sortOrderValue.Length - 2);
      }
      else
      {
        sortOrder = sortOrderValue;
      }
    }

    // Check if this is a literal array or a variable
    if (arraySort.Array is VariableExpression varExpr)
    {
      // It's a variable reference
      var arrayName = varExpr.Name;
      var arrayType = GetArrayType(arrayName);

      string sortCommand;
      if (arrayType == "number")
      {
        // Numeric sort
        sortCommand = sortOrder == "desc" ? "sort -nr" : "sort -n";
      }
      else if (arrayType == "boolean")
      {
        // Boolean sort: false (0) before true (1)
        if (sortOrder == "desc")
        {
          sortCommand = "sed 's/true/1/g; s/false/0/g' | sort -nr | sed 's/1/true/g; s/0/false/g'";
        }
        else
        {
          sortCommand = "sed 's/true/1/g; s/false/0/g' | sort -n | sed 's/1/true/g; s/0/false/g'";
        }
      }
      else
      {
        // String sort (lexicographic)
        sortCommand = sortOrder == "desc" ? "sort -r" : "sort";
      }

      return $"$({uniqueVar}=(); while IFS= read -r line; do {uniqueVar}+=(\"$line\"); done < <(printf '%s\\n' \"${{{arrayName}[@]}}\" | {sortCommand}); echo \"${{{uniqueVar}[@]}}\")";
    }
    else
    {
      // It's an array literal or expression result - use simple lexicographic sort
      var sortCommand = sortOrder == "desc" ? "sort -r" : "sort";

      // If it's an array literal, we need to create a temporary array first
      if (arraySort.Array is ArrayLiteral)
      {
        return $"$({uniqueVar}={compiledArray}; tmp=(); while IFS= read -r line; do tmp+=(\"$line\"); done < <(printf '%s\\n' \"${{{uniqueVar}[@]}}\" | {sortCommand}); echo \"${{tmp[@]}}\")";
      }
      else
      {
        return $"$({uniqueVar}=(); while IFS= read -r line; do {uniqueVar}+=(\"$line\"); done < <(printf '%s\\n' {compiledArray} | {sortCommand}); echo \"${{{uniqueVar}[@]}}\")";
      }
    }
  }

  private string CompileArrayMergeExpression(ArrayMergeExpression arrayMerge)
  {
    var arrayName1 = ExtractVariableName(CompileExpression(arrayMerge.Array1));
    var arrayName2 = ExtractVariableName(CompileExpression(arrayMerge.Array2));

    // Create a new array by combining both arrays using printf to output each element
    return $"($(printf '%s\\n' \"${{{arrayName1}[@]}}\" \"${{{arrayName2}[@]}}\"))";
  }

  private string CompileArrayShuffleExpression(ArrayShuffleExpression arrayShuffle)
  {
    var compiledArray = CompileExpression(arrayShuffle.Array);

    // Check if this is a literal array or a variable
    if (arrayShuffle.Array is VariableExpression varExpr)
    {
      // It's a variable reference
      var arrayName = varExpr.Name;
      return $"($(if command -v shuf &> /dev/null; then printf '%s\\n' \"${{{arrayName}[@]}}\" | shuf; else arr=(\"${{{arrayName}[@]}}\"); for ((i=${{#arr[@]}}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp=\"${{arr[i]}}\"; arr[i]=\"${{arr[j]}}\"; arr[j]=\"$temp\"; done; printf '%s\\n' \"${{arr[@]}}\"; fi))";
    }
    else
    {
      // It's an array literal or expression result
      return $"($(arr={compiledArray}; if command -v shuf &> /dev/null; then printf '%s\\n' \"${{arr[@]}}\" | shuf; else for ((i=${{#arr[@]}}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp=\"${{arr[i]}}\"; arr[i]=\"${{arr[j]}}\"; arr[j]=\"$temp\"; done; printf '%s\\n' \"${{arr[@]}}\"; fi))";
    }
  }

  private string GetArrayType(string arrayName)
  {
    // For now, we'll use a simple heuristic based on common naming patterns
    // In a full implementation, we'd track variable types through the compilation process

    // Check for common number array names
    if (arrayName.Contains("number") || arrayName.Contains("num") || arrayName.Contains("int") ||
        arrayName.Contains("float") || arrayName.Contains("double") || arrayName.Contains("single"))
    {
      return "number";
    }

    // Check for common boolean array names
    if (arrayName.Contains("boolean") || arrayName.Contains("bool") || arrayName.Contains("flag") ||
        arrayName.Contains("flags") || arrayName.Contains("switch"))
    {
      return "boolean";
    }

    // Default to string for most arrays
    return "string";
  }

  private string CompileFsDirnameExpression(FsDirnameExpression fsDirname)
  {
    var path = CompileExpression(fsDirname.Path);
    return $"$(dirname {path})";
  }

  private string CompileFsFileNameExpression(FsFileNameExpression fsFileName)
  {
    var path = CompileExpression(fsFileName.Path);
    return $"$(basename {path})";
  }

  private string CompileFsExtensionExpression(FsExtensionExpression fsExtension)
  {
    // Handle string literals specially - extract extension directly
    if (fsExtension.Path is LiteralExpression literal && literal.Type == "string")
    {
      // For string literals, we can extract the extension directly
      var fileName = literal.Value;
      var dotIndex = fileName.LastIndexOf('.');
      if (dotIndex >= 0 && dotIndex < fileName.Length - 1)
      {
        var extension = fileName.Substring(dotIndex + 1);
        return $"\"{extension}\"";
      }
      else
      {
        return "\"\""; // No extension
      }
    }

    // For variables, use bash parameter expansion
    var path = CompileExpression(fsExtension.Path);
    // Extract variable name if it's in ${var} format
    var varName = ExtractVariableName(path);
    return $"\"${{{varName}##*.}}\"";
  }

  private string CompileFsReadFileExpression(FsReadFileExpression fsReadFile)
  {
    var filePath = CompileExpression(fsReadFile.FilePath);
    return $"$(cat {filePath})";
  }

  private string CompileFsCopyExpression(FsCopyExpression fsCopy)
  {
    var sourcePath = CompileExpression(fsCopy.SourcePath);
    var targetPath = CompileExpression(fsCopy.TargetPath);
    return $"$(mkdir -p $(dirname {targetPath}) && cp -r {sourcePath} {targetPath} && echo \"true\" || echo \"false\")";
  }

  private string CompileFsMoveExpression(FsMoveExpression fsMove)
  {
    var sourcePath = CompileExpression(fsMove.SourcePath);
    var targetPath = CompileExpression(fsMove.TargetPath);
    return $"$(mkdir -p $(dirname {targetPath}) && mv {sourcePath} {targetPath} && echo \"true\" || echo \"false\")";
  }

  private string CompileFsRenameExpression(FsRenameExpression fsRename)
  {
    var oldName = CompileExpression(fsRename.OldName);
    var newName = CompileExpression(fsRename.NewName);
    return $"$(mv {oldName} {newName} && echo \"true\" || echo \"false\")";
  }

  private string CompileFsDeleteExpression(FsDeleteExpression fsDelete)
  {
    var path = CompileExpression(fsDelete.Path);
    return $"$(rm -rf {path} && echo \"true\" || echo \"false\")";
  }

  private string CompileFsFindExpression(FsFindExpression fsFind)
  {
    var searchPath = CompileExpression(fsFind.SearchPath);
    
    // Remove quotes if already quoted, then add our own quotes for bash safety
    if (searchPath.StartsWith("\"") && searchPath.EndsWith("\""))
    {
      searchPath = searchPath[1..^1]; // Remove existing quotes
    }
    
    var findCmd = $"find \"{searchPath}\"";
    
    if (fsFind.NamePattern != null)
    {
      var namePattern = CompileExpression(fsFind.NamePattern);
      
      // Remove quotes if already quoted, then add our own quotes for bash safety
      if (namePattern.StartsWith("\"") && namePattern.EndsWith("\""))
      {
        namePattern = namePattern[1..^1]; // Remove existing quotes
      }
      
      findCmd += $" -name \"{namePattern}\"";
    }
    
    // Return as array split by newlines, filtering empty entries
    return $"$(IFS=$'\\n'; mapfile -t _utah_find_results < <({findCmd} 2>/dev/null); printf '%s\\n' \"${{_utah_find_results[@]}}\")";
  }

  private string CompileFsParentDirNameExpression(FsParentDirNameExpression fsParentDirName)
  {
    var path = CompileExpression(fsParentDirName.Path);
    return $"$(basename $(dirname {path}))";
  }

  private string CompileFsExistsExpression(FsExistsExpression fsExists)
  {
    var path = CompileExpression(fsExists.Path);
    return $"$([ -e {path} ] && echo \"true\" || echo \"false\")";
  }

  private string CompileFsCreateTempFolderExpression(FsCreateTempFolderExpression expr)
  {
    // Prepare optional inputs
    var prefixExpr = expr.Prefix != null ? CompileExpression(expr.Prefix) : null;
    var baseDirExpr = expr.BaseDir != null ? CompileExpression(expr.BaseDir) : null;

    // Build bash script ensuring mktemp-first with secure fallback
    // We inline the variables and sanitize prefix at runtime for dynamic inputs
    var setPrefix = prefixExpr != null ? $"_utah_prefix={prefixExpr};" : "_utah_prefix=utah;";
    var setBase = baseDirExpr != null ? $"_utah_tmp_base={baseDirExpr};" : "_utah_tmp_base=\"${TMPDIR:-/tmp}\";";

    var script =
      "$({ " +
      setBase + " " +
      setPrefix + " " +
      "_utah_prefix=$(echo \"${_utah_prefix}\" | tr -cd '[:alnum:]_.-'); " +
      "[ -z \"${_utah_prefix}\" ] && _utah_prefix=utah; " +
      "if command -v mktemp >/dev/null 2>&1; then " +
      (baseDirExpr != null
        ? "dir=$(mktemp -d -p \"${_utah_tmp_base}\" \"${_utah_prefix}.XXXXXXXX\" 2>/dev/null) || dir=$(mktemp -d \"${_utah_tmp_base%/}/${_utah_prefix}.XXXXXXXX\" 2>/dev/null); "
        : "dir=$(mktemp -d -t \"${_utah_prefix}.XXXXXXXX\" 2>/dev/null) || dir=$(mktemp -d \"${_utah_tmp_base%/}/${_utah_prefix}.XXXXXXXX\" 2>/dev/null); ") +
      "fi; " +
      "if [ -z \"${dir}\" ]; then for _i in 1 2 3 4 5 6 7 8 9 10; do _suf=$(LC_ALL=C tr -dc 'a-z0-9' </dev/urandom | head -c12); [ -z \"${_suf}\" ] && _suf=$$; _cand=\"${_utah_tmp_base%/}/${_utah_prefix}-${_suf}\"; if mkdir -m 700 \"${_cand}\" 2>/dev/null; then dir=\"${_cand}\"; break; fi; done; fi; " +
      "if [ -z \"${dir}\" ]; then echo \"Error: Could not create temporary directory\" >&2; exit 1; fi; " +
      "echo \"${dir}\"; })";

    return script;
  }

  private string CompileStringInterpolationExpression(StringInterpolationExpression stringInterpolation)
  {
    var parts = new List<string>();
    foreach (var part in stringInterpolation.Parts)
    {
      if (part is string str)
      {
        parts.Add(str);
      }
      else if (part is Expression expr)
      {
        var compiled = CompileExpression(expr);
        // Remove quotes if it's a string literal
        if (compiled.StartsWith("\"") && compiled.EndsWith("\""))
        {
          compiled = compiled[1..^1];
        }
        
        // If the compiled expression already contains bash substitution (starts with $), use it directly
        if (compiled.StartsWith("$"))
        {
          parts.Add(compiled);
        }
        else
        {
          parts.Add($"${{{ExtractVariableName(compiled)}}}");
        }
      }
    }
    return $"\"{string.Join("", parts)}\"";
  }

  private string CompileTimerStopExpression()
  {
    // timer.stop() generates the end timer, calculates elapsed time, and returns it
    return "(_utah_timer_end=$(date +%s%3N); echo $((_utah_timer_end - _utah_timer_start)))";
  }

  private string CompileTimerCurrentExpression(TimerCurrentExpression timerCurrent)
  {
    // Current timer value is the time elapsed since timer start
    return "$(( $(date +%s%3N) - _utah_timer_start ))";
  }

  private string CompileArgsHasExpression(ArgsHasExpression argsHas)
  {
    return $"$(__utah_has_arg \"{argsHas.Flag}\" \"$@\" && echo \"true\" || echo \"false\")";
  }

  private string CompileArgsGetExpression(ArgsGetExpression argsGet)
  {
    return "$(__utah_get_arg \"" + argsGet.Flag + "\" \"$@\")";
  }

  private string CompileArgsAllExpression(ArgsAllExpression argsAll)
  {
    return "$(__utah_all_args \"$@\")";
  }

  private string CompileStringNamespaceCallExpression(StringNamespaceCallExpression stringCall)
  {
    var functionName = stringCall.FunctionName;
    var args = stringCall.Arguments.Select(CompileExpression).ToList();

    return functionName switch
    {
      "length" => CompileStringLengthFunction(args),
      "trim" => CompileStringTrimFunction(args),
      "toUpperCase" => CompileStringToUpperCaseFunction(args),
      "toLowerCase" => CompileStringToLowerCaseFunction(args),
      "startsWith" => CompileStringStartsWithFunction(args),
      "endsWith" => CompileStringEndsWithFunction(args),
      "includes" => CompileStringIncludesFunction(args),
      "replace" => CompileStringReplaceFunction(args),
      "replaceAll" => CompileStringReplaceAllFunction(args),
      "split" => CompileStringSplitFunction(args),
      "substring" => CompileStringSubstringFunction(args),
      "slice" => CompileStringSliceFunction(args),
      "indexOf" => CompileStringIndexOfFunction(args),
      "padStart" => CompileStringPadStartFunction(args),
      "padEnd" => CompileStringPadEndFunction(args),
      "repeat" => CompileStringRepeatFunction(args),
      "capitalize" => CompileStringCapitalizeFunction(args),
      "isEmpty" => CompileStringIsEmptyFunction(args),
      _ => throw new NotSupportedException($"String function '{functionName}' is not supported")
    };
  }

  private string CompileArrayNamespaceCallExpression(ArrayNamespaceCallExpression arrayCall)
  {
    var functionName = arrayCall.FunctionName;
    var args = arrayCall.Arguments.Select(CompileExpression).ToList();

    return functionName switch
    {
      "length" => CompileArrayLengthFunction(args),
      "isEmpty" => CompileArrayIsEmptyFunction(args),
      "contains" => CompileArrayContainsFunction(args),
      "reverse" => CompileArrayReverseFunction(args),
      "push" => CompileArrayPushFunction(args),
      "join" => CompileArrayJoinFunction(args),
      "sort" => CompileArraySortFunction(args),
      "merge" => CompileArrayMergeFunction(args),
      "shuffle" => CompileArrayShuffleFunction(args),
      _ => throw new NotSupportedException($"Array function '{functionName}' is not supported")
    };
  }

  private string CompileStringLengthFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.length() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"\"${{#{varName}}}\"";
  }

  private string CompileStringTrimFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.trim() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"\"$(echo \"${{{varName}}}\" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')\"";
  }

  private string CompileStringToUpperCaseFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.toUpperCase() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"\"${{{varName}^^}}\"";
  }

  private string CompileStringToLowerCaseFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.toLowerCase() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"\"${{{varName},,}}\"";
  }

  private string CompileStringStartsWithFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.startsWith() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var prefixStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    return $"$([[ \"${{{varName}}}\" == {prefixStr}* ]] && echo \"true\" || echo \"false\")";
  }

  private string CompileStringEndsWithFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.endsWith() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var suffixStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    return $"$([[ \"${{{varName}}}\" == *{suffixStr} ]] && echo \"true\" || echo \"false\")";
  }

  private string CompileStringIncludesFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.includes() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var searchStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    return $"$(case \"${{{varName}}}\" in *{searchStr}*) echo \"true\";; *) echo \"false\";; esac)";
  }

  private string CompileStringReplaceFunction(List<string> args)
  {
    if (args.Count != 3)
      throw new InvalidOperationException("string.replace() requires exactly 3 arguments");

    var varName = ExtractVariableName(args[0]);
    var searchStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    var replaceStr = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];
    return $"\"${{{varName}/{searchStr}/{replaceStr}}}\"";
  }

  private string CompileStringReplaceAllFunction(List<string> args)
  {
    if (args.Count != 3)
      throw new InvalidOperationException("string.replaceAll() requires exactly 3 arguments");

    var target = args[0];
    var searchStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    var replaceStr = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];

    // If target is a string literal, we need to handle it differently
    if (target.StartsWith("\"") && target.EndsWith("\""))
    {
      // For string literals, use sed or other bash tools
      var literalValue = target[1..^1]; // Remove quotes
      return $"\"$(echo \"{literalValue}\" | sed \"s/{searchStr}/{replaceStr}/g\")\"";
    }
    else
    {
      // For variables, use bash parameter expansion
      var varName = ExtractVariableName(target);
      return $"\"${{{varName}//{searchStr}/{replaceStr}}}\"";
    }
  }

  private string GetStringValueForBashExpansion(string arg)
  {
    // If it's a string literal, return the literal content for direct bash expansion
    if (arg.StartsWith("\"") && arg.EndsWith("\""))
    {
      return arg[1..^1]; // Remove quotes for bash parameter expansion
    }
    // If it's a variable reference, extract the variable name
    else if (arg.StartsWith("${") && arg.EndsWith("}"))
    {
      var varName = arg[2..^1];
      return varName; // Return just the variable name for ${var//search/replace} expansion
    }
    else
    {
      // Assume it's a variable name
      return arg;
    }
  }

  private string CompileStringSplitFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.split() requires exactly 2 arguments");

    // Handle the string to split - can be a literal or variable
    var stringToSplit = args[0];
    string targetValue;

    if (stringToSplit.StartsWith("\"") && stringToSplit.EndsWith("\""))
    {
      // It's a literal string - remove quotes for the here-string
      targetValue = stringToSplit[1..^1];
    }
    else
    {
      // It's a variable reference - use variable expansion
      var varName = ExtractVariableName(stringToSplit);
      targetValue = $"${{{varName}}}";
    }

    var separatorStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    return $"$(IFS='{separatorStr}'; read -ra SPLIT_ARRAY <<< \"{targetValue}\"; echo \"${{SPLIT_ARRAY[@]}}\")";
  }

  private string CompileStringSubstringFunction(List<string> args)
  {
    if (args.Count < 2 || args.Count > 3)
      throw new InvalidOperationException("string.substring() requires 2 or 3 arguments");

    var target = args[0];
    var startValue = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];

    // Handle string literals vs variables
    if (target.StartsWith("\"") && target.EndsWith("\""))
    {
      var literalValue = target[1..^1]; // Remove quotes
      if (args.Count == 2)
      {
        return $"\"$(echo \"{literalValue}\" | cut -c{startValue}-)\"";
      }
      else
      {
        var lengthValue = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];
        var endPos = $"$(({startValue} + {lengthValue} - 1))";
        return $"\"$(echo \"{literalValue}\" | cut -c{startValue}-{endPos})\"";
      }
    }
    else
    {
      var varName = ExtractVariableName(target);
      if (args.Count == 2)
      {
        return $"\"${{{varName}:{startValue}}}\"";
      }
      else
      {
        var lengthValue = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];
        return $"\"${{{varName}:{startValue}:{lengthValue}}}\"";
      }
    }
  }

  private string CompileStringSliceFunction(List<string> args)
  {
    if (args.Count < 2 || args.Count > 3)
      throw new InvalidOperationException("string.slice() requires 2 or 3 arguments");

    var target = args[0];
    var startValue = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];

    // Handle string literals vs variables
    if (target.StartsWith("\"") && target.EndsWith("\""))
    {
      var literalValue = target[1..^1]; // Remove quotes
      if (args.Count == 2)
      {
        return $"\"$(echo \"{literalValue}\" | cut -c{startValue}-)\"";
      }
      else
      {
        var endValue = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];
        return $"\"$(echo \"{literalValue}\" | cut -c{startValue}-{endValue})\"";
      }
    }
    else
    {
      var varName = ExtractVariableName(target);
      if (args.Count == 2)
      {
        return $"\"${{{varName}:{startValue}}}\"";
      }
      else
      {
        var endValue = args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2];
        return $"\"$(echo \"${{{varName}}}\" | cut -c{startValue}-{endValue})\"";
      }
    }
  }

  private string CompileStringIndexOfFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.indexOf() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var searchStr = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    return $"$(temp=\"${{{varName}}}\"; pos=${{temp%%{searchStr}*}}; [[ \"$pos\" == \"${{{varName}}}\" ]] && echo \"-1\" || echo \"${{#pos}}\")";
  }

  private string CompileStringPadStartFunction(List<string> args)
  {
    if (args.Count < 2 || args.Count > 3)
      throw new InvalidOperationException("string.padStart() requires 2 or 3 arguments");

    var targetValue = args[0];
    var lengthValue = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    var padChar = args.Count == 3 ? (args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2]) : " ";

    return $"\"$(printf \"%*s\" {lengthValue} {targetValue} | sed \"s/ /{padChar}/g\")\"";
  }

  private string CompileStringPadEndFunction(List<string> args)
  {
    if (args.Count < 2 || args.Count > 3)
      throw new InvalidOperationException("string.padEnd() requires 2 or 3 arguments");

    var targetValue = args[0];
    var lengthValue = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];
    var padChar = args.Count == 3 ? (args[2].StartsWith("\"") && args[2].EndsWith("\"") ? args[2][1..^1] : args[2]) : " ";

    return $"\"$(printf \"%-*s\" {lengthValue} {targetValue} | sed \"s/ /{padChar}/g\")\"";
  }

  private string CompileStringRepeatFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("string.repeat() requires exactly 2 arguments");

    var targetValue = args[0];
    var countValue = args[1].StartsWith("\"") && args[1].EndsWith("\"") ? args[1][1..^1] : args[1];

    return $"\"$(for ((i=0; i<{countValue}; i++)); do printf \"%s\" {targetValue}; done)\"";
  }

  private string CompileStringCapitalizeFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.capitalize() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"\"$(echo \"${{{varName}}}\" | sed 's/^./\\U&/')\"";
  }

  private string CompileStringIsEmptyFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("string.isEmpty() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"$([[ -z \"$(echo \"${{{varName}}}\" | xargs)\" ]] && echo \"true\" || echo \"false\")";
  }

  private string EnsureQuoted(string value)
  {
    // If it's already quoted or is a literal number, don't add quotes
    if (value.StartsWith("\"") && value.EndsWith("\""))
      return value;
    if (IsNumericLiteralValue(value))
      return value;

    // For variable expressions like ${var}, wrap in quotes
    return $"\"{value}\"";
  }

  private bool IsNumericLiteralValue(string value)
  {
    return double.TryParse(value, out _);
  }

  private string CompileSchedulerCronExpression(SchedulerCronExpression schedulerCron)
  {
    var cronPattern = CompileExpression(schedulerCron.CronPattern);

    // Remove quotes from cron pattern if it's a string literal
    if (cronPattern.StartsWith("\"") && cronPattern.EndsWith("\""))
    {
      cronPattern = cronPattern[1..^1];
    }

    // Generate unique script name and directory
    var lines = new List<string>
    {
      "# Create Utah cron directory",
      "_utah_cron_dir=\"$HOME/.utah/cron\"",
      "mkdir -p \"${_utah_cron_dir}\"",
      $"_utah_cron_script=\"${{_utah_cron_dir}}/job_$(date +%s)_$$.sh\"",
      "",
      "# Generate the cron job script",
      "cat > \"${_utah_cron_script}\" << 'EOF'",
      "#!/bin/bash",
      $"# Generated by Utah - scheduler.cron(\"{cronPattern}\")"
    };

    // Compile the lambda body
    foreach (var stmt in schedulerCron.Job.Body)
    {
      lines.AddRange(CompileStatement(stmt));
    }

    lines.AddRange(new[]
    {
      "EOF",
      "",
      "# Make the script executable",
      "chmod +x \"${_utah_cron_script}\"",
      "",
      "# Check if similar cron job already exists",
      $"_utah_cron_pattern=\"{cronPattern.Replace("*", "\\*")}.*utah.*job_\"",
      "if ! crontab -l 2>/dev/null | grep -q \"${_utah_cron_pattern}\"; then",
      "    # Add to crontab",
      $"    (crontab -l 2>/dev/null; echo \"{cronPattern} ${{_utah_cron_script}}\") | crontab -",
      $"    echo \"Cron job installed: {cronPattern} ${{_utah_cron_script}}\"",
      "else",
      "    echo \"Similar cron job already exists\"",
      "fi"
    });

    // Return as a compound statement that can be used in expressions
    return $"$({string.Join("; ", lines)})";
  }

  private string CompileLambdaExpression(LambdaExpression lambda)
  {
    // Lambda expressions are typically compiled as part of their parent construct
    // For now, we'll throw an error if someone tries to use a lambda outside of supported contexts
    throw new InvalidOperationException("Lambda expressions can only be used in specific contexts like scheduler.cron()");
  }

  private string CompileJsonParseExpression(JsonParseExpression jsonParse)
  {
    var jsonString = CompileExpression(jsonParse.JsonString);
    // For json.parse(), we store the parsed JSON in a variable that can be referenced later
    var varName = ExtractVariableName(jsonString);
    return $"$(echo {jsonString} | jq .)";
  }

  private string CompileJsonStringifyExpression(JsonStringifyExpression jsonStringify)
  {
    var jsonObject = CompileExpression(jsonStringify.JsonObject);
    // Extract variable name if it's in ${var} format, otherwise use as-is
    var varName = ExtractVariableName(jsonObject);
    return $"$(echo \"${{{varName}}}\" | jq -c .)";
  }

  private string CompileJsonIsValidExpression(JsonIsValidExpression jsonIsValid)
  {
    var jsonString = CompileExpression(jsonIsValid.JsonString);
    return $"$(echo {jsonString} | jq empty >/dev/null 2>&1 && echo \"true\" || echo \"false\")";
  }

  private string CompileJsonGetExpression(JsonGetExpression jsonGet)
  {
    var jsonObject = CompileExpression(jsonGet.JsonObject);
    var path = CompileExpression(jsonGet.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(jsonObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | jq -r '{pathValue}')";
  }

  private string CompileJsonSetExpression(JsonSetExpression jsonSet)
  {
    var jsonObject = CompileExpression(jsonSet.JsonObject);
    var path = CompileExpression(jsonSet.Path);
    var value = CompileExpression(jsonSet.Value);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(jsonObject);
    var pathValue = path.Trim('"');

    // Handle different value types
    if (value.StartsWith("\"") && value.EndsWith("\""))
    {
      // String value - keep quotes for jq
      return $"$(echo \"${{{varName}}}\" | jq '{pathValue} = {value}')";
    }
    else if (value == "true" || value == "false" || (int.TryParse(value, out _)) || (double.TryParse(value, out _)))
    {
      // Boolean or numeric value - no quotes
      return $"$(echo \"${{{varName}}}\" | jq '{pathValue} = {value}')";
    }
    else
    {
      // Variable reference - use --arg
      var valueVarName = ExtractVariableName(value);
      return $"$(echo \"${{{varName}}}\" | jq --arg val \"${{{valueVarName}}}\" '{pathValue} = $val')";
    }
  }

  private string CompileJsonHasExpression(JsonHasExpression jsonHas)
  {
    var jsonObject = CompileExpression(jsonHas.JsonObject);
    var path = CompileExpression(jsonHas.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(jsonObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | jq 'try {pathValue} catch false | type != \"null\"' | tr '[:upper:]' '[:lower:]')";
  }

  private string CompileJsonDeleteExpression(JsonDeleteExpression jsonDelete)
  {
    var jsonObject = CompileExpression(jsonDelete.JsonObject);
    var path = CompileExpression(jsonDelete.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(jsonObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | jq 'del({pathValue})')";
  }

  private string CompileJsonKeysExpression(JsonKeysExpression jsonKeys)
  {
    var jsonObject = CompileExpression(jsonKeys.JsonObject);
    var varName = ExtractVariableName(jsonObject);

    return $"$(echo \"${{{varName}}}\" | jq -r 'keys[]')";
  }

  private string CompileJsonValuesExpression(JsonValuesExpression jsonValues)
  {
    var jsonObject = CompileExpression(jsonValues.JsonObject);
    var varName = ExtractVariableName(jsonObject);

    return $"$(echo \"${{{varName}}}\" | jq -r '.[]')";
  }

  private string CompileJsonMergeExpression(JsonMergeExpression jsonMerge)
  {
    var jsonObject1 = CompileExpression(jsonMerge.JsonObject1);
    var jsonObject2 = CompileExpression(jsonMerge.JsonObject2);

    var varName1 = ExtractVariableName(jsonObject1);
    var varName2 = ExtractVariableName(jsonObject2);

    return $"$(echo \"${{{varName1}}}\" | jq --argjson obj2 \"${{{varName2}}}\" '. * $obj2')";
  }

  private string CompileJsonInstallDependenciesExpression(JsonInstallDependenciesExpression jsonInstallDependencies)
  {
    return @"$(
if ! command -v jq &> /dev/null; then
  echo ""Installing jq for JSON processing...""
  if command -v apt-get &> /dev/null; then
    sudo apt-get update && sudo apt-get install -y jq
  elif command -v yum &> /dev/null; then
    sudo yum install -y jq
  elif command -v dnf &> /dev/null; then
    sudo dnf install -y jq
  elif command -v brew &> /dev/null; then
    brew install jq
  elif command -v pacman &> /dev/null; then
    sudo pacman -S --noconfirm jq
  else
    echo ""Error: Unable to install jq. Please install it manually.""
    exit 1
  fi
  if command -v jq &> /dev/null; then
    echo ""jq installed successfully""
  else
    echo ""Error: jq installation failed""
    exit 1
  fi
else
  echo ""jq is already installed""
fi
)";
  }

  private string CompileYamlParseExpression(YamlParseExpression yamlParse)
  {
    var yamlString = CompileExpression(yamlParse.YamlString);
    // For yaml.parse(), we convert YAML to JSON internally for easier manipulation
    return $"$(echo {yamlString} | yq -o=json .)";
  }

  private string CompileYamlStringifyExpression(YamlStringifyExpression yamlStringify)
  {
    var yamlObject = CompileExpression(yamlStringify.YamlObject);
    // Extract variable name if it's in ${var} format, otherwise use as-is
    var varName = ExtractVariableName(yamlObject);
    return $"$(echo \"${{{varName}}}\" | yq -o=yaml .)";
  }

  private string CompileYamlIsValidExpression(YamlIsValidExpression yamlIsValid)
  {
    var yamlString = CompileExpression(yamlIsValid.YamlString);
    return $"$(echo {yamlString} | yq empty >/dev/null 2>&1 && echo \"true\" || echo \"false\")";
  }

  private string CompileYamlGetExpression(YamlGetExpression yamlGet)
  {
    var yamlObject = CompileExpression(yamlGet.YamlObject);
    var path = CompileExpression(yamlGet.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(yamlObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq -r '{pathValue}')";
  }

  private string CompileYamlSetExpression(YamlSetExpression yamlSet)
  {
    var yamlObject = CompileExpression(yamlSet.YamlObject);
    var path = CompileExpression(yamlSet.Path);
    var value = CompileExpression(yamlSet.Value);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(yamlObject);
    var pathValue = path.Trim('"');

    // Handle different value types
    if (value.StartsWith("\"") && value.EndsWith("\""))
    {
      // String value - keep quotes for yq
      return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq '{pathValue} = {value}' | yq -o=yaml .)";
    }
    else if (value == "true" || value == "false" || (int.TryParse(value, out _)) || (double.TryParse(value, out _)))
    {
      // Boolean or numeric value - no quotes
      return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq '{pathValue} = {value}' | yq -o=yaml .)";
    }
    else
    {
      // Variable reference - use --arg
      var valueVarName = ExtractVariableName(value);
      return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq --arg val \"${{{valueVarName}}}\" '{pathValue} = $val' | yq -o=yaml .)";
    }
  }

  private string CompileYamlHasExpression(YamlHasExpression yamlHas)
  {
    var yamlObject = CompileExpression(yamlHas.YamlObject);
    var path = CompileExpression(yamlHas.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(yamlObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq 'try {pathValue} catch false | type != \"null\"' | tr '[:upper:]' '[:lower:]')";
  }

  private string CompileYamlDeleteExpression(YamlDeleteExpression yamlDelete)
  {
    var yamlObject = CompileExpression(yamlDelete.YamlObject);
    var path = CompileExpression(yamlDelete.Path);

    // Extract variable name and remove quotes from path
    var varName = ExtractVariableName(yamlObject);
    var pathValue = path.Trim('"');

    return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq 'del({pathValue})' | yq -o=yaml .)";
  }

  private string CompileYamlKeysExpression(YamlKeysExpression yamlKeys)
  {
    var yamlObject = CompileExpression(yamlKeys.YamlObject);
    var varName = ExtractVariableName(yamlObject);

    return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq -r 'keys[]')";
  }

  private string CompileYamlValuesExpression(YamlValuesExpression yamlValues)
  {
    var yamlObject = CompileExpression(yamlValues.YamlObject);
    var varName = ExtractVariableName(yamlObject);

    return $"$(echo \"${{{varName}}}\" | yq -o=json . | jq -r '.[]')";
  }

  private string CompileYamlMergeExpression(YamlMergeExpression yamlMerge)
  {
    var yamlObject1 = CompileExpression(yamlMerge.YamlObject1);
    var yamlObject2 = CompileExpression(yamlMerge.YamlObject2);

    var varName1 = ExtractVariableName(yamlObject1);
    var varName2 = ExtractVariableName(yamlObject2);

    return $"$(echo \"${{{varName1}}}\" | yq -o=json . | jq --argjson obj2 \"$(echo \\\"${{{varName2}}}\\\" | yq -o=json .)\" '. * $obj2' | yq -o=yaml .)";
  }

  private string CompileYamlInstallDependenciesExpression(YamlInstallDependenciesExpression yamlInstallDependencies)
  {
    return @"$(
if ! command -v yq &> /dev/null; then
  echo ""Installing yq for YAML processing...""
  if command -v snap &> /dev/null; then
    sudo snap install yq
  elif command -v brew &> /dev/null; then
    brew install yq
  elif command -v wget &> /dev/null; then
    sudo wget -qO /usr/local/bin/yq https://github.com/mikefarah/yq/releases/latest/download/yq_linux_amd64
    sudo chmod +x /usr/local/bin/yq
  else
    echo ""Error: Unable to install yq. Please install it manually.""
    exit 1
  fi
  if command -v yq &> /dev/null; then
    echo ""yq installed successfully""
  else
    echo ""Error: yq installation failed""
    exit 1
  fi
else
  echo ""yq is already installed""
fi
if ! command -v jq &> /dev/null; then
  echo ""Installing jq for JSON processing (required by YAML functions)...""
  if command -v apt-get &> /dev/null; then
    sudo apt-get update && sudo apt-get install -y jq
  elif command -v yum &> /dev/null; then
    sudo yum install -y jq
  elif command -v dnf &> /dev/null; then
    sudo dnf install -y jq
  elif command -v brew &> /dev/null; then
    brew install jq
  elif command -v pacman &> /dev/null; then
    sudo pacman -S --noconfirm jq
  else
    echo ""Error: Unable to install jq. Please install it manually.""
    exit 1
  fi
  if command -v jq &> /dev/null; then
    echo ""jq installed successfully""
  else
    echo ""Error: jq installation failed""
    exit 1
  fi
else
  echo ""jq is already installed""
fi
)";
  }

  private static int _uniqueIdCounter = 0;
  private static int GetUniqueId()
  {
    return ++_uniqueIdCounter;
  }

  private string CompileTemplateUpdateExpression(TemplateUpdateExpression templateUpdate)
  {
    // Generate bash code for template.update(sourceFile, targetFile)
    // This uses envsubst to replace ${VAR} tokens with environment variables
    var sourceFile = CompileExpression(templateUpdate.SourceFilePath);
    var targetFile = CompileExpression(templateUpdate.TargetFilePath);

    var uniqueId = GetUniqueId();
    var resultVar = $"_utah_template_result_{uniqueId}";

    return $"$({resultVar}=$(envsubst < {sourceFile} > {targetFile} && echo \"true\" || echo \"false\"); echo ${{{resultVar}}})";
  }

  // Array namespace function implementations
  private string CompileArrayLengthFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("array.length() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"${{#{varName}[@]}}";
  }

  private string CompileArrayIsEmptyFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("array.isEmpty() requires exactly 1 argument");

    var arrayArg = args[0];

    // Handle command substitutions that generate arrays
    if (arrayArg.StartsWith("$("))
    {
      var uniqueVar = $"_utah_isempty_{GetUniqueId()}";
      return $"$({uniqueVar}=({arrayArg}); [ ${{#{uniqueVar}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
    }
    else
    {
      // Handle regular variable names
      var varName = ExtractVariableName(arrayArg);
      return $"$([ ${{#{varName}[@]}} -eq 0 ] && echo \"true\" || echo \"false\")";
    }
  }

  private string CompileArrayContainsFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("array.contains() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var item = args[1];

    // Remove quotes from item if it's a string literal for comparison
    var itemValue = item.StartsWith("\"") && item.EndsWith("\"") ? item[1..^1] : item;

    return $"$(case \" ${{{varName}[@]}} \" in *\" {itemValue} \"*) echo \"true\" ;; *) echo \"false\" ;; esac)";
  }

  private string CompileArrayReverseFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("array.reverse() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);
    return $"($(for ((i=${{#{varName}[@]}}-1; i>=0; i--)); do echo \"${{{varName}[i]}}\"; done))";
  }

  private string CompileArrayPushFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("array.push() requires exactly 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var item = args[1];

    // array.push() modifies the original array in-place
    return $"{varName}+=({item})";
  }

  private string CompileArrayJoinFunction(List<string> args)
  {
    if (args.Count < 1 || args.Count > 2)
      throw new InvalidOperationException("array.join() requires 1 or 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var separator = args.Count == 2 ? args[1] : "\",\"";  // Default to comma

    // Remove quotes from separator if it's a string literal
    if (separator.StartsWith("\"") && separator.EndsWith("\""))
    {
      separator = separator[1..^1];
    }

    if (string.IsNullOrEmpty(separator))
    {
      // No separator - concatenate directly
      return $"$(printf '%s' \"${{{varName}[@]}}\")";
    }
    else if (separator == ",")
    {
      // Special case for comma - use printf with format
      return $"$(IFS=','; printf '%s' \"${{{varName}[*]}}\")";
    }
    else
    {
      // For other separators, use printf with the separator
      var escapedSeparator = separator.Replace("'", "'\"'\"'"); // Escape single quotes
      return $"$(IFS='{escapedSeparator}'; printf '%s' \"${{{varName}[*]}}\")";
    }
  }

  private string CompileArraySortFunction(List<string> args)
  {
    if (args.Count < 1 || args.Count > 2)
      throw new InvalidOperationException("array.sort() requires 1 or 2 arguments");

    var varName = ExtractVariableName(args[0]);
    var sortOrder = args.Count == 2 ? args[1] : "\"asc\"";

    // Remove quotes from sort order if it's a string literal
    if (sortOrder.StartsWith("\"") && sortOrder.EndsWith("\""))
    {
      sortOrder = sortOrder[1..^1];
    }

    if (sortOrder == "desc")
    {
      return $"($(printf '%s\\n' \"${{{varName}[@]}}\" | sort -r))";
    }
    else
    {
      return $"($(printf '%s\\n' \"${{{varName}[@]}}\" | sort))";
    }
  }

  private string CompileArrayMergeFunction(List<string> args)
  {
    if (args.Count != 2)
      throw new InvalidOperationException("array.merge() requires exactly 2 arguments");

    var varName1 = ExtractVariableName(args[0]);
    var varName2 = ExtractVariableName(args[1]);

    // Create a new array by combining both arrays using printf to output each element
    return $"($(printf '%s\\n' \"${{{varName1}[@]}}\" \"${{{varName2}[@]}}\"))";
  }

  private string CompileArrayShuffleFunction(List<string> args)
  {
    if (args.Count != 1)
      throw new InvalidOperationException("array.shuffle() requires exactly 1 argument");

    var varName = ExtractVariableName(args[0]);

    // Use shuf command if available, otherwise fall back to Fisher-Yates shuffle using RANDOM
    return $"($(if command -v shuf &> /dev/null; then printf '%s\\n' \"${{{varName}[@]}}\" | shuf; else arr=(\"${{{varName}[@]}}\"); for ((i=${{#arr[@]}}-1; i>0; i--)); do j=$((RANDOM % (i+1))); temp=\"${{arr[i]}}\"; arr[i]=\"${{arr[j]}}\"; arr[j]=\"$temp\"; done; printf '%s\\n' \"${{arr[@]}}\"; fi))";
  }
}
