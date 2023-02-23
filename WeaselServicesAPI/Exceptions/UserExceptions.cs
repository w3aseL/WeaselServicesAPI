namespace WeaselServicesAPI.Exceptions
{
    public class UserExistsException : ApplicationException
    {
        public UserExistsException(string? message) : base(message) { }
    }

    public class UserNotFoundException : ApplicationException
    {
        public UserNotFoundException(string? message) : base(message) { }
    }
}
