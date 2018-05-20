using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class TrackerException : Exception{
        public TrackerException(string message) : base(message){
        }
    }
}