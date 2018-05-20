using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class VerbXApiException : XApiException{

        public VerbXApiException(string message) : base(message){
        }
    }
}