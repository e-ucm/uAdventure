using System;

namespace AssetPackage.Exceptions
{
    public class ActorXApiException : XApiException {
        public ActorXApiException(string message) : base(message){
        }
    }
}