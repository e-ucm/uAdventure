namespace Xasu.Requests
{
    public static class HttpStatus
    {
        // xAPI Recommended: https://github.com/adlnet/xAPI-Spec/blob/master/xAPI-Communication.md#details-12
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int Conflict = 409;
        public const int PreconditionFailed = 412;
        public const int RequestEntityTooLarge = 413;
        public const int TooManyRequests = 429;
        public const int InternalServerError = 500;
        public const int NotImplemented = 501;
        public const int BadGateway = 502;
        public const int ServiceUnavailable = 503;
        public const int GatewayTimeout = 504;
    }
}
