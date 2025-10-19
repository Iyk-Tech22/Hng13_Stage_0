# Profile API

## Overview
The Profile API is an ASP.NET Core Web API controller that provides user profile information along with an interesting cat fact from an external API.
The API implements rate limiting and follows RESTful principles.

## Features
- User Profile Endpoint: Returns user profile information such as email, name, stack.
- External Service Integration: Retrieve random fact from [CatFact.Ninja](https://catfact.ninja/)
- Rate Limiting: Implements rate limiting to prevent abuse of server.
- Error Handling: Implements standard global error handling to server proper user friendly errors
- Standardized Responses: Consistent API response format

## API Endpoints

### Get Profile
Retrieve user informations, along with random cat fact.

#### Endpoint
GET api/me

#### Response
- 200 OK: Successfully retrieved profile and cat fact
- 400 Bad Request: Invalid request
- 500 Internal Server Error: Server error or external API failure

#### Response Schema
```
{
  "status": "success",
  "user": {
    "email": "string",
    "name": "string",
    "stack": "string"
  },
  "timeStamp": "datetime",
  "fact": "string"
}
```
#### Example Response
```
{
  "status": "success",
  "user": {
    "email": "ikechukwugodwin22@gmail.com",
    "name": "Ikechukwu F. Godwin",
    "stack": "C#/ASP.NET Web API"
  },
  "timeStamp": "2023-10-15T14:30:00Z",
  "fact": "Cats have whiskers on the backs of their front legs as well."
}
```
## Dependencies
### NuGet Packages
- Microsoft.AspNetCore.Mvc
- Microsoft.AspNetCore.RateLimiting
- Newtonsoft.Json

## External Services
- CatFact API: https://catfact.ninja/fact

## Development

### Prerequisites
- .NET 6.0 or later
- ASP.NET Core Runtime
- HTTP client for testing (Postman, curl, etc.)

### Running the API
- Clone the repository
- Restore NuGet packages: dotnet restore
- Run the application: dotnet run
- Access the endpoint: GET https://localhost:{port}/api/me

## Testing
Test the endpoint using curl:
```curl -X GET "https://localhost:{port}/api/me" -H "accept: application/json"
```
