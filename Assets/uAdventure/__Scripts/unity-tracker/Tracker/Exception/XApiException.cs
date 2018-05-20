using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class XApiException : TrackerException{

        public XApiException(string message) : base(message){
        }
    }
}