/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace AssetPackage
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for web service response.
    /// </summary>
    ///
    /// <remarks>
    /// Implemented by assets requesting result notification of a
    /// IWebServiceRequest.
    /// </remarks>
    [Obsolete("Use IWebServiceRequest instead")]
    public interface IWebServiceResponseAsync
    {
        /// <summary>
        /// Called when a WebRequest results in an Error.
        /// </summary>
        ///
        /// <param name="url"> URL of the document. </param>
        /// <param name="msg"> The error message. </param>
        void Error(string url, string msg);

        /// <summary>
        /// Called after a Successfull WebRequest (no Exceptions).
        /// </summary>
        ///
        /// <param name="url">     URL of the document. </param>
        /// <param name="code">    The code. </param>
        /// <param name="headers"> The headers. </param>
        /// <param name="body">    The body. </param>
        void Success(string url, int code, Dictionary<string, string> headers, string body);
    }

    /// <summary>
    /// Interface for web service request.
    /// </summary>
    ///
    /// <remarks>
    /// Implemented on a Bridge.
    /// Will be replaced by the code from IWebServiceRequest2 once tested.
    /// </remarks>
    public interface IWebServiceRequestAsync
    {

#warning Add Tag or Data parameter to this call so we can identify it in IWebServiceResponse?

        /// <summary>
        /// Web service request.
        /// </summary>
        ///
        /// <param name="method">      The method. </param>
        /// <param name="uri">         URI of the document. </param>
        /// <param name="headers">     The headers. </param>
        /// <param name="body">        The body. </param>
        /// <param name="response">    The response. </param>
        void WebServiceRequestAsync(
            string method,
            Uri uri,
            Dictionary<string, string> headers,
            string body,
            IWebServiceResponseAsync response
            );
    }
}
