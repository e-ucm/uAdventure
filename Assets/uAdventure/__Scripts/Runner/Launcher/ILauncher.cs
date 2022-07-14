using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAdventure.Runner
{
    public interface ILauncher
    {
        int Priority { get; }

        bool WantsControl();

        bool Launch();

    }
}
