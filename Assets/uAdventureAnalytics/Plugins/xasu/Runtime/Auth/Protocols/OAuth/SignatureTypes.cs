using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xasu.Auth.Protocols.OAuth
{

    /// <summary>
    /// Provides a predefined set of algorithms that are supported officially by the protocol
    /// </summary>
    public enum SignatureTypes
    {
        HMACSHA1,
        PLAINTEXT,
        RSASHA1
    }
}
