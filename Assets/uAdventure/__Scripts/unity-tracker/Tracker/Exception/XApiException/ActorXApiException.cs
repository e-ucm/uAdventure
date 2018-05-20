using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class ActorXApiException : XApiException {

        public ActorXApiException(string message) : base(message){
        }
    }
}