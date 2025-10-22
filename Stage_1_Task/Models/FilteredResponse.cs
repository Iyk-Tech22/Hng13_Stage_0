namespace Stage_1_Task.Models
{
    public class FilteredResponse
    {
        public List<APIResponse> Data { get; set; } = new List<APIResponse>();
        public int Count { get; set; }
        public object Filters_applied { get; set; }
    }
}