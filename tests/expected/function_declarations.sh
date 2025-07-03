greet() {
  local name="$1"
  echo "$"Hello, " + name + "!""
}
add() {
  local a="$1"
  local b="$2"
  echo "$a + b"
}
result="greet("Utah")"
echo $result
sum="add(5, 3)"
echo Sum: "$sum"
