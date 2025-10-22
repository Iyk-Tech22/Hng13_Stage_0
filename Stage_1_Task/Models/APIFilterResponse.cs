namespace Stage_1_Task.Models
{
    public class APIFilterResponse
    {
        List<APIResponse> Data { get; set; }
        public int counts { get; set; }
        public object Filter_applied { get; set; }
    }
}
