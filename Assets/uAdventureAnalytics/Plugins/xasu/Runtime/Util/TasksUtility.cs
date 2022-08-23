using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xasu.Util
{
    internal static class TasksUtility
    {
        public static async void WrapErrors(this Task task)
        {
            await task;
        }
    }
}
