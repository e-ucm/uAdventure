using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uAdventure.Runner
{
    public class LaunchManager
    {

        private List<ILauncher> launchers;

        public void AddLauncher(ILauncher launcher)
        {
            launchers.Add(launcher);
            // Sort by priority number descending
            launchers.Sort((l1, l2) => l1.Priority - l2.Priority); 
        }

        public void RemoveLauncher(ILauncher launcher)
        {
            if (launchers.Contains(launcher))
            {
                launchers.Remove(launcher);
            }
        }

        public bool Launch()
        {
            bool launched = false;
            foreach(var launcher in launchers)
            {
                if (launcher.WantsControl() && launcher.Launch())
                {
                    launched = true;
                    break;
                }
            }
            return launched;
        }
    }
}
