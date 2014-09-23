using ArmaBrowser.Logic.DefaultImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaBrowser.Logic
{
    internal static class LogicManager
    {
        public static ILogicContext CreateNewLogicContext()
        {
            return new LogicContext();
        }
    }
}
