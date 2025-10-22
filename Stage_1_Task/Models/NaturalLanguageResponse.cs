namespace Stage_1_Task.Models
{
    public class NaturalLanguageResponse
    {
        public List<APIResponse> Data { get; set; } = new List<APIResponse>();
        public int Count { get; set; }
        public InterpretedQuery Interpreted_query { get; set; }
    }

    public class InterpretedQuery
    {
        public string Original { get; set; }
        public ParsedFilters Parsed_filters { get; set; }
    }

    public class ParsedFilters
    {
        public bool? Is_palindrome { get; set; }
        public int? Min_length { get; set; }
        public int? Max_length { get; set; }
        public int? Word_count { get; set; }
        public string Contains_character { get; set; }
    }
}