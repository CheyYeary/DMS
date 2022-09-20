# Set User Secrets

At the root of the project open terminal and run the following:

- ```dotnet user-secrets init```
- ```dotnet user-secrets set "ConnectionStrings:dev" "my_connectionstring”```


### Example Sign up request
header: 
F9168C5E-CEB2-4faa-B6BF-329BF39FA1E4

body:
{
  "deadManSwitchInterval": "00:01:10",
  "recurrence": {
    "frequency": "Year",
    "interval": 1,
    "startTime": "2022-09-20T22:15:31.897Z",
    "endTime": "2023-09-20T22:15:31.897Z",
    "timeZone": "UTC"
  }
}