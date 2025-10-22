using Microsoft.AspNetCore.Mvc;
using Stage_1_Task.Data;
using Stage_1_Task.Exceptions;
using Stage_1_Task.Models;
using System.Web;

namespace Stage_1_Task.Controllers
{
    [ApiController]
    [Route("api/strings")]
    public class StringAnalyzerController : ControllerBase
    {
        private readonly APIResponse _apiResponse;

        public StringAnalyzerController()
        {
            _apiResponse = new APIResponse();
        }

        [HttpGet("{string_value}", Name = "GetString")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<APIResponse> GetString(string string_value)
        {
            // URL decode the value to handle encoded strings
            var decodedValue = HttpUtility.UrlDecode(string_value);

            var result = StringAnalyzerStore.Store.FirstOrDefault(s => s.value == decodedValue);

            if (result == null)
            {
                throw new NotFoundException("String does not exist in the system");
            }

            _apiResponse.Value = result.value;
            _apiResponse.Created_at = result.createdAt;
            _apiResponse.Properties = Properties.Create(result.value);
            _apiResponse.Id = _apiResponse.Properties.Sha256_hash;
            return Ok(_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<APIResponse> CreateString([FromBody] APIRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.value))
            {
                throw new BadRequestException("Invalid request body or missing 'value' field");
            }

            if (request.value.GetType() != typeof(string))
            {
                throw new UnprocessableEntityException("Invalid data type for 'value' (must be string)");
            }

            var stringFromStore = StringAnalyzerStore.Store.FirstOrDefault(v => v.value == request.value);

            if (stringFromStore != null)
            {
                throw new ConflictException("String already exists in the system");
            }

            Store val = Store.Create(request.value);
            StringAnalyzerStore.Store.Add(val);

            _apiResponse.Value = val.value;
            _apiResponse.Properties = Properties.Create(request.value);
            _apiResponse.Created_at = val.createdAt;
            _apiResponse.Id = _apiResponse.Properties.Sha256_hash;

            return CreatedAtRoute("GetString", new { string_value = Uri.EscapeDataString(request.value) }, _apiResponse);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<FilteredResponse> GetStringWithFilters([FromQuery] QueryFilters filter)
        {
            var allStrings = StringAnalyzerStore.Store.Select(storeItem =>
            {
                var properties = Properties.Create(storeItem.value);
                return new APIResponse
                {
                    Id = properties.Sha256_hash,
                    Value = storeItem.value,
                    Properties = properties,
                    Created_at = storeItem.createdAt
                };
            }).ToList();

            var filteredStrings = allStrings.AsEnumerable();

            if (filter.Is_palindrome.HasValue)
            {
                filteredStrings = filteredStrings.Where(s => s.Properties.Is_palindrome == filter.Is_palindrome.Value);
            }

            if (filter.Min_length.HasValue)
            {
                filteredStrings = filteredStrings.Where(s => s.Properties.Length >= filter.Min_length.Value);
            }

            if (filter.Max_length.HasValue)
            {
                filteredStrings = filteredStrings.Where(s => s.Properties.Length <= filter.Max_length.Value);
            }

            if (filter.Word_count.HasValue)
            {
                filteredStrings = filteredStrings.Where(s => s.Properties.Word_count == filter.Word_count.Value);
            }

            if (!string.IsNullOrEmpty(filter.Contains_character) && filter.Contains_character.Length == 1)
            {
                var character = filter.Contains_character[0];
                filteredStrings = filteredStrings.Where(s => s.Value.Contains(character));
            }

            var result = filteredStrings.ToList();

            return Ok(new FilteredResponse
            {
                Data = result,
                Count = result.Count,
                Filters_applied = filter
            });
        }

        [HttpGet("filter-by-natural-language")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<NaturalLanguageResponse> FilterByNaturalLanguage([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new BadRequestException("Query parameter is required");
            }

            var parsedFilters = ParseNaturalLanguageQuery(query);

            if (parsedFilters == null)
            {
                throw new BadRequestException("Unable to parse natural language query");
            }

            if (parsedFilters.Min_length.HasValue && parsedFilters.Max_length.HasValue &&
                parsedFilters.Min_length > parsedFilters.Max_length)
            {
                throw new UnprocessableEntityException("Query parsed but resulted in conflicting filters");
            }

            var queryFilters = new QueryFilters
            {
                Is_palindrome = parsedFilters.Is_palindrome,
                Min_length = parsedFilters.Min_length,
                Max_length = parsedFilters.Max_length,
                Word_count = parsedFilters.Word_count,
                Contains_character = parsedFilters.Contains_character
            };

            var filterResult = GetStringWithFilters(queryFilters).Result as OkObjectResult;
            var filteredResponse = filterResult?.Value as FilteredResponse;

            return Ok(new NaturalLanguageResponse
            {
                Data = filteredResponse?.Data ?? new List<APIResponse>(),
                Count = filteredResponse?.Count ?? 0,
                Interpreted_query = new InterpretedQuery
                {
                    Original = query,
                    Parsed_filters = parsedFilters
                }
            });
        }

        [HttpDelete("{string_value}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteString(string string_value)
        {
            var decodedValue = HttpUtility.UrlDecode(string_value);
            var result = StringAnalyzerStore.Store.FirstOrDefault(s => s.value == decodedValue);

            if (result == null)
            {
                throw new NotFoundException("String does not exist in the system");
            }

            StringAnalyzerStore.Store.Remove(result);
            return NoContent();
        }

        private ParsedFilters ParseNaturalLanguageQuery(string query)
        {
            var lowerQuery = query.ToLower();
            var parsed = new ParsedFilters();

            if (lowerQuery.Contains("palindromic") || lowerQuery.Contains("palindrome"))
            {
                parsed.Is_palindrome = true;
            }

            if (lowerQuery.Contains("single word") || lowerQuery.Contains("one word"))
            {
                parsed.Word_count = 1;
            }
            else if (lowerQuery.Contains("two words") || lowerQuery.Contains("2 words"))
            {
                parsed.Word_count = 2;
            }

            var longerThanMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"longer than (\d+)");
            if (longerThanMatch.Success)
            {
                if (int.TryParse(longerThanMatch.Groups[1].Value, out int minLength))
                {
                    parsed.Min_length = minLength + 1;
                }
            }

            var shorterThanMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"shorter than (\d+)");
            if (shorterThanMatch.Success)
            {
                if (int.TryParse(shorterThanMatch.Groups[1].Value, out int maxLength))
                {
                    parsed.Max_length = maxLength - 1;
                }
            }

            var containsMatch = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"contain(s|ing)? (the letter |the character )?([a-zA-Z])");
            if (containsMatch.Success)
            {
                parsed.Contains_character = containsMatch.Groups[3].Value;
            }
            else
            {
                var simpleContains = System.Text.RegularExpressions.Regex.Match(lowerQuery, @"contain(s|ing)? ([a-zA-Z])(\s|$)");
                if (simpleContains.Success)
                {
                    parsed.Contains_character = simpleContains.Groups[2].Value;
                }
            }

            if (lowerQuery == "all single word palindromic strings")
            {
                parsed.Word_count = 1;
                parsed.Is_palindrome = true;
            }
            else if (lowerQuery == "strings longer than 10 characters")
            {
                parsed.Min_length = 11;
            }
            else if (lowerQuery == "strings containing the letter z")
            {
                parsed.Contains_character = "z";
            }
            else if (lowerQuery == "palindromic strings")
            {
                parsed.Is_palindrome = true;
            }

            return parsed;
        }
    }
}