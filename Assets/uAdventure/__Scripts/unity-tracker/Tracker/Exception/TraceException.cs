using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class TraceException : TrackerException{
        public TraceException(string message) : base(message){
        }
    }
}