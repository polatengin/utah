#!/bin/bash

user=$(jq -cn '{"name": "Ada", "age": 32, "active": true, "address": {"city": "Ankara", "zip": 6800}, "tags": ["builder", "maintainer"], "metadata": {"team": "compiler", "tier": "core"}}')
echo "User name:"
echo "$(echo "${user}" | jq -cr '.name')"
echo "User city:"
echo "$(echo "${user}" | jq -cr '.address.city')"
echo "User tier:"
echo "$(echo "${user}" | jq -cr '.metadata.tier')"
userPayload=$(echo '{"name":"Grace","age":37,"active":true,"address":{"city":"Istanbul","zip":34000},"tags":["platform","infra"],"metadata":{"team":"runtime","tier":"infra"}}' | jq .)
validatedUser=$(_utah_schema_value_2="${userPayload}"; if echo "${_utah_schema_value_2}" | jq -ce 'type == "object" and ((keys | sort) == ["active", "address", "age", "metadata", "name", "tags"]) and (.name | type == "string") and (.age | type == "number") and (.active | type == "boolean") and (.address | type == "object" and ((keys | sort) == ["city", "zip"]) and (.city | type == "string") and (.zip | type == "number")) and (.tags | type == "array" and all(.[]; type == "string")) and (.metadata | type == "object" and all(.[]; type == "string"))' >/dev/null 2>&1; then echo "${_utah_schema_value_2}"; else echo "Schema validation failed for User" >&2; exit 1; fi)
echo "Validated user:"
echo "$(echo "${validatedUser}" | jq -cr '.name')"
echo "$(echo "${validatedUser}" | jq -cr '.address.city')"
people=("${user}" "${validatedUser}")
names=($(_utah_map_source_3=("${people[@]}"); _utah_map_result_3=(); for ((_utah_map_index_3=0; _utah_map_index_3<${#_utah_map_source_3[@]}; _utah_map_index_3++)); do person="${_utah_map_source_3[_utah_map_index_3]}"; _utah_map_value_3=$(echo "$(echo "${person}" | jq -cr '.name')"); _utah_map_result_3+=("${_utah_map_value_3}"); done; printf '%s\n' "${_utah_map_result_3[@]}"))
echo "People:"
echo "$(IFS=', '; printf '%s' "${names[*]}")"
