using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class ExtensionXApiException : XApiException{

        public ExtensionXApiException(string message) : base(message){
        }
    }
}