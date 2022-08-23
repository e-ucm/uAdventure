using System;

namespace Xasu.Exceptions
{
    public class XApiException : TrackerException{

        public XApiException(string message) : base(message){
        }
    }
}