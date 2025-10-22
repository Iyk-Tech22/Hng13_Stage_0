namespace Stage_1_Task.Models
{
    public record Store(string value, DateTime createdAt)
    {
        private Store(string value): this(value, DateTime.UtcNow) { }
        public static Store Create(string value) => new Store(value);
    }
}
