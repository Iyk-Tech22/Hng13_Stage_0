namespace Stage_1_Task.Exceptions
{
    public class BadRequestException: BaseException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
