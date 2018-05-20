using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class TargetXApiException : XApiException{

        public TargetXApiException(string message) : base(message){
        }
    }
}