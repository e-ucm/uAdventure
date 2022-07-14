using System;
using System.Collections.Generic;
using Xasu.Auth.Protocols;
using Polly;

namespace Xasu.Requests
{
    public class MyHttpRequest
    {
        public string url { get; set; }
        public String method { get; set; }
        public String resource { get; set; }
        public Dictionary<String, String> queryParams { get; set; } = new Dictionary<String, String>();
        public Dictionary<String, String> headers { get; set; } = new Dictionary<String, String>();
        public String contentType { get; set; }
        public byte[] content { get; set; }
        public IAuthProtocol authorization { get; set; }
        public IAsyncPolicy policy { get; set; }
    }
}
