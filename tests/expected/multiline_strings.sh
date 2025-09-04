#!/bin/bash

basicMultiline=$'\nThis is a basic\nmultiline string\nthat spans multiple lines\n'
echo "Basic multiline:"
echo "${basicMultiline}"
sqlQuery=$'\nSELECT id, name, email\nFROM users\nWHERE active = 1\nORDER BY name ASC\n'
echo "SQL Query:"
echo "${sqlQuery}"
tableName="customers"
condition="status='active'"
columns="id, name, email"
interpolatedQuery=$'\nSELECT '${columns}$'\nFROM '${tableName}$'\nWHERE '${condition}$'\nORDER BY last_login DESC\n'
echo "Interpolated Query:"
echo "${interpolatedQuery}"
appName="MyApp"
version="1.0.0"
port=8080
configTemplate=$'\n{\n"name": "'${appName}$'",\n"version": "'${version}$'",\n"server": {\n"port": '${port}$',\n"host": "localhost"\n}\n}\n'
echo "Config Template:"
echo "${configTemplate}"
scriptContent=$'\n#!/bin/bash\nset -e\necho "Starting deployment..."\necho "Deployment complete!"\n'
echo "Script Content:"
echo "${scriptContent}"
env="staging"
errorMessage=$'\nError: Configuration validation failed\nThe following issues were found:\n- Missing required field database.host\n- Invalid port number: must be between 1 and 65535\n- Unsupported environment: '${env}$'\nPlease check your configuration and try again.\n'
echo "Error Message:"
echo "${errorMessage}"
title="Welcome"
content="Hello World"
htmlTemplate=$'\n<!DOCTYPE html>\n<html>\n<head>\n<title>'${title}$'</title>\n</head>\n<body>\n<h1>'${title}$'</h1>\n<p>'${content}$'</p>\n</body>\n</html>\n'
echo "HTML Template:"
echo "${htmlTemplate}"
emptyMultiline=$'\n'
echo "Empty multiline length:"
echo "${#emptyMultiline}"
singleInMultiline=$'Just one line'
echo "Single line in multiline:"
echo "${singleInMultiline}"
specialChars=$'\nSpecial characters test:\n- Quotes work fine\n- Backslashes work too\n- Tabs and spaces\n- Mixed content is supported\n'
echo "Special characters:"
echo "${specialChars}"
