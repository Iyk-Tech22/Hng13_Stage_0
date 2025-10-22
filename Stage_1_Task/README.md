# String Analyzer API

## Overview
The String Analyzer API is an ASP.NET Core Web API that provides comprehensive string analysis capabilities. It computes various string properties including palindrome detection, character frequency analysis, and cryptographic hashing. The API implements proper RESTful principles with complete CRUD operations and advanced filtering capabilities.

## Features
- **String Analysis**: Compute comprehensive properties including length, palindrome status, unique characters, word count, SHA-256 hash, and character frequency
- **Natural Language Processing**: Advanced filtering using natural language queries
- **Duplicate Prevention**: Ensures string uniqueness with proper conflict handling
- **Flexible Filtering**: Multiple filtering options including palindrome status, length ranges, word count, and character containment
- **RESTful Design**: Proper HTTP status codes and standardized response formats
- **Error Handling**: Comprehensive error handling with user-friendly messages
- **Rate Limiting Ready**: Structure supports rate limiting implementation

## API Endpoints

### 1. Create/Analyze String
Create a new string analysis with comprehensive property computation.

**Endpoint:** `POST /api/strings`

**Request Body:**
```json
{
  "value": "string to analyze"
}
```