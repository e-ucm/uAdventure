using System;

namespace AssetPackage.Exceptions
{
    public class ExtensionException : TrackerException{
        public ExtensionException(string message) : base(message){
        }
	}
}