#!/bin/bash

allItems=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#allItems[@]} items in current directory"
markdownFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." -name "*.md" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#markdownFiles[@]} markdown files"
testFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "tests" -name "*.shx" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#testFiles[@]} test files"
expectedFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "tests/expected" -name "*.sh" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#expectedFiles[@]} expected output files"
configFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." -name "*.json" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
searchDir="src"
pattern="*.cs"
csharpFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "${searchDir}" -name "${pattern}" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#csharpFiles[@]} C# files in ${searchDir}"
nonExistent=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." -name "*.nonexistent" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Non-existent files: ${#nonExistent[@]}"
for file in "${markdownFiles[@]}"; do
  if [ "$(echo "${file}" | sed 's/^[[:space:]]*//;s/[[:space:]]*$//')" != "" ]; then
    echo "Markdown file: ${file}"
  fi
done
txtFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." -name "*.txt" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
logFiles=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "." -name "*.log" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Text files: ${#txtFiles[@]}"
echo "Log files: ${#logFiles[@]}"
baseDir="tests"
filePattern="fs_*.shx"
findResults=$(IFS=$'\n'; mapfile -t _utah_find_results < <(find "${baseDir}" -name "${filePattern}" 2>/dev/null); printf '%s\n' "${_utah_find_results[@]}")
echo "Found ${#findResults[@]} fs test files"
