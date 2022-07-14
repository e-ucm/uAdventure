using System;
using UnityEngine.Networking;

namespace Xasu.Exceptions
{
    [Serializable]
    public class APIException : Exception
    {
        public int HttpCode { get; private set; }
        public UnityWebRequest Request { get; private set; }

        public APIException(int httpCode, string message, UnityWebRequest request) : base(message)
        {
            this.HttpCode = httpCode;
            this.Request = request;
        }

        public APIException(int httpCode, string message, UnityWebRequest request, Exception innerException) : base(message, innerException)
        {
            this.HttpCode = httpCode;
            this.Request = request;
        }
    }
}
