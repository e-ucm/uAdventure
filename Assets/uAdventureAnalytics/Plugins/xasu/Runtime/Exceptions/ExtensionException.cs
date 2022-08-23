using System;

namespace Xasu.Exceptions
{
    public class ExtensionException : TrackerException {
        public ExtensionException(string message) : base(message){
        }
	}
}