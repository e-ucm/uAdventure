using UnityEngine;
using System;

namespace RAGE.Analytics.Exceptions{

    public class ExtensionException : TrackerException{
        public ExtensionException(string message) : base(message){
        }
	}
}