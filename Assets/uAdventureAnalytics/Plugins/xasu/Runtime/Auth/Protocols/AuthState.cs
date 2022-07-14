using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xasu.Auth.Protocols
{
    public enum AuthState
    {
        Working, RequiresInteraction, Errored
    }
}
