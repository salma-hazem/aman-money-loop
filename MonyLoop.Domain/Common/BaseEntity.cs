namespace Mony_Loop.Application.Common
{
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized
    }

    public class Error
    {
        public string Code { get; }
        public string Message { get; }
        public ErrorType Type { get; }

        private Error(string code, string message, ErrorType type)
        {
            Code = code;
            Message = message;
            Type = type;
        }

        public static Error Validation(string code, string message) =>
            new(code, message, ErrorType.Validation);

        public static Error NotFound(string code, string message) =>
            new(code, message, ErrorType.NotFound);

        public static Error Conflict(string code, string message) =>
            new(code, message, ErrorType.Conflict);

        public static Error Unauthorized(string code, string message) =>
            new(code, message, ErrorType.Unauthorized);
    }
}