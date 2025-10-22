namespace Stage_0_Task.Models
{
    public class APIResponse
    {
        public string Status { get; set; }
        public UserModel User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Fact { get; set; }
    }
}
