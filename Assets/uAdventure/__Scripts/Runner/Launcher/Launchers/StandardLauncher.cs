using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace uAdventure.Runner
{
    public class StandardLauncher : ILauncher
    {
        public int Priority { get { return 0; } }
        
        public virtual bool WantsControl()
        {
            return true;
        }
        
        public virtual bool Launch()
        {
            
        }
    }
}
