using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class ValueExtensionException : ExtensionException{
        public ValueExtensionException(string message) : base(message){
        }
    }
}