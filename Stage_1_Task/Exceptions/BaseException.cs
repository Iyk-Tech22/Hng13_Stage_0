namespace Stage_1_Task.Exceptions
{
    public abstract class BaseException : Exception
    {
        protected BaseException(string message)
        {
            Message = message;
        }

        private string Message { get; set; }

    }
}
