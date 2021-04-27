namespace Connecterra.StateTracking.Common.Core
{
    public class Result<T>
    {
        public Result(T value)
        {
            IsSuccess = true;
            Value = value;
        }
        public Result(string errorMessage)
        {
            IsSuccess = false;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get;}
        public bool IsError { get { return !IsSuccess; } }
        public string ErrorMessage { get; }
        public T Value { get;}

    }
}
