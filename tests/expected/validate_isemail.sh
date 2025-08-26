#!/bin/bash

email1="user@example.com"
isValid1=$(echo ${email1} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Basic email: ${email1} is  + isValid1))"
email2="first.last@subdomain.example.co.uk"
isValid2=$(echo ${email2} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Complex domain: ${email2} is  + isValid2))"
email3="user+tag@example.org"
isValid3=$(echo ${email3} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Plus tag: ${email3} is  + isValid3))"
email4="user_name@example-site.com"
isValid4=$(echo ${email4} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Underscore and hyphen: ${email4} is  + isValid4))"
invalidEmail1="invalid.email"
isInvalid1=$(echo ${invalidEmail1} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((No @ symbol: ${invalidEmail1} is  + isInvalid1))"
invalidEmail2="@example.com"
isInvalid2=$(echo ${invalidEmail2} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((No local part: ${invalidEmail2} is  + isInvalid2))"
invalidEmail3="user@"
isInvalid3=$(echo ${invalidEmail3} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((No domain: ${invalidEmail3} is  + isInvalid3))"
invalidEmail4="user@.com"
isInvalid4=$(echo ${invalidEmail4} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Invalid domain: ${invalidEmail4} is  + isInvalid4))"
if [ $(echo "admin@company.com" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false") ]; then
  echo "Admin email is valid"
else
  echo "Admin email is invalid"
fi
userInput="test@email.com"
if [ $(echo ${userInput} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false") ]; then
  echo "User provided valid email"
else
  echo "User provided invalid email"
fi
emailCheckResult=$(echo "contact@business.org" | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "Contact email validation result: ${emailCheckResult}"
configEmail="config@app.local"
isValidConfig=$(echo ${configEmail} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
if [ "${isValidConfig}" = "true" ]; then
  echo "Configuration email is valid"
else
  echo "Configuration email needs correction"
fi
edgeCase1="a@b.co"
edgeValid1=$(echo ${edgeCase1} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Short email: ${edgeCase1} is  + edgeValid1))"
edgeCase2="very.long.email.address@very.long.domain.name.example.com"
edgeValid2=$(echo ${edgeCase2} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Long email: ${edgeCase2} is  + edgeValid2))"
numericEmail="user123@123domain.com"
numericValid=$(echo ${numericEmail} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Numeric email: ${numericEmail} is  + numericValid))"
specialEmail="test.email!#$%&*+-/=?^_`{|}~@example.com"
specialValid=$(echo ${specialEmail} | grep -qE '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$' && echo "true" || echo "false")
echo "$((Special chars: ${specialEmail} is  + specialValid))"
