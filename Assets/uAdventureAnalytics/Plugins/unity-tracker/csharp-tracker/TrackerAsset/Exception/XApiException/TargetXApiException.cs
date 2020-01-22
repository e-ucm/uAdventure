using System;

namespace AssetPackage.Exceptions
{
    public class TargetXApiException : XApiException{
        public TargetXApiException(string message) : base(message){
        }
    }
}