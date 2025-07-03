IFS=',' read -ra items <<< "one,two,three"
echo "Using for-in loop:"
for item in "${items[@]}"; do
  echo "Item: ${item}"
done
echo "Using traditional for loop:"
i=0
while [ $i -lt 3 ]; do
  echo "Index ${i}: ${items[i]}"
  i=$((i + 1))
done
