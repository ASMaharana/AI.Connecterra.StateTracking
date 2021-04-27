namespace Connecterra.StateTracking.Common.Core
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public bool IsError { get; private set; }
        public string ErrorMessage { get; private set; }
        
        public static Result OK()
        {
            return new Result{ IsSuccess = true };
        }

        public static Result Error(string message)
        {
            return new Result { IsError = true, ErrorMessage =  message};
        }

        public static Result<T> OK<T>(T value)
        {
            return new Result<T>(value);
        }

        public static Result<T> Error<T>(string message)
        {
            return new Result<T>(message);
        }
    }
}
