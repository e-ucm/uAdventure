using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class KeyExtensionException : ExtensionException{
        public KeyExtensionException(string message) : base(message){
        }
    }
}