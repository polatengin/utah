#!/bin/bash

url1="https://www.example.com"
isValid1=$(echo ${url1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Basic HTTPS: ${url1} is ${isValid1}"
url2="http://example.com"
isValid2=$(echo ${url2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Basic HTTP: ${url2} is ${isValid2}"
url3="ftp://files.example.com"
isValid3=$(echo ${url3} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "FTP protocol: ${url3} is ${isValid3}"
url4="file:///home/user/document.txt"
isValid4=$(echo ${url4} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "File protocol: ${url4} is ${isValid4}"
urlPort1="https://api.example.com:8080"
isValidPort1=$(echo ${urlPort1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "HTTPS with port: ${urlPort1} is ${isValidPort1}"
urlPort2="http://localhost:3000"
isValidPort2=$(echo ${urlPort2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "HTTP localhost: ${urlPort2} is ${isValidPort2}"
urlPath1="https://www.example.com/path/to/resource"
isValidPath1=$(echo ${urlPath1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "With path: ${urlPath1} is ${isValidPath1}"
urlPath2="https://api.example.com/v1/users/123"
isValidPath2=$(echo ${urlPath2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "API path: ${urlPath2} is ${isValidPath2}"
urlQuery1="https://search.example.com?q=utah&type=language"
isValidQuery1=$(echo ${urlQuery1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "With query: ${urlQuery1} is ${isValidQuery1}"
urlQuery2="https://api.example.com/search?term=test&limit=10"
isValidQuery2=$(echo ${urlQuery2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Multiple params: ${urlQuery2} is ${isValidQuery2}"
urlFragment1="https://docs.example.com/guide#section1"
isValidFragment1=$(echo ${urlFragment1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "With fragment: ${urlFragment1} is ${isValidFragment1}"
urlFragment2="https://app.example.com/dashboard?tab=overview#settings"
isValidFragment2=$(echo ${urlFragment2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Query + fragment: ${urlFragment2} is ${isValidFragment2}"
complexUrl1="https://sub.domain.example.co.uk:8443/api/v2/resources?filter=active&sort=name#results"
isValidComplex1=$(echo ${complexUrl1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Complex URL: ${complexUrl1} is ${isValidComplex1}"
invalidUrl1="not-a-url"
isInvalid1=$(echo ${invalidUrl1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Not a URL: ${invalidUrl1} is ${isInvalid1}"
invalidUrl2="httpexample.com"
isInvalid2=$(echo ${invalidUrl2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Missing ://: ${invalidUrl2} is ${isInvalid2}"
invalidUrl3="https://"
isInvalid3=$(echo ${invalidUrl3} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "No domain: ${invalidUrl3} is ${isInvalid3}"
invalidUrl4="smtp://mail.example.com"
isInvalid4=$(echo ${invalidUrl4} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Unsupported protocol: ${invalidUrl4} is ${isInvalid4}"
invalidUrl5="https://.example.com"
isInvalid5=$(echo ${invalidUrl5} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Invalid domain start: ${invalidUrl5} is ${isInvalid5}"
if [ $(echo "https://api.company.com" | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false") ]; then
  echo "API URL is valid"
else
  echo "API URL is invalid"
fi
apiEndpoint="https://jsonplaceholder.typicode.com/posts"
if [ $(echo ${apiEndpoint} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false") ]; then
  echo "API endpoint is valid"
else
  echo "API endpoint is invalid"
fi
urlCheckResult=$(echo "https://github.com/polatengin/utah" | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "GitHub URL validation result: ${urlCheckResult}"
configUrl="https://config.app.local:9000/settings"
isValidConfig=$(echo ${configUrl} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
if [ "${isValidConfig}" = "true" ]; then
  echo "Configuration URL is valid"
else
  echo "Configuration URL needs correction"
fi
edgeCase1="http://a.b"
edgeValid1=$(echo ${edgeCase1} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Short URL: ${edgeCase1} is ${edgeValid1}"
edgeCase2="https://very.long.subdomain.very.long.domain.example.com:8080/very/long/path/to/resource.html?very=long&query=parameters&with=multiple&values=here#verylongfragment"
edgeValid2=$(echo ${edgeCase2} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Long URL: ${edgeCase2} is ${edgeValid2}"
numericUrl="https://192.168.1.100:8080"
numericValid=$(echo ${numericUrl} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "IP URL: ${numericUrl} is ${numericValid}"
specialUrl="https://api-v2.example-site.com/user_profile"
specialValid=$(echo ${specialUrl} | grep -qE '^(https?|ftp|file)://[A-Za-z0-9.-]+(:[0-9]+)?(/[^?#]*)?([?][^#]*)?([#].*)?$' && echo "true" || echo "false")
echo "Special chars: ${specialUrl} is ${specialValid}"
