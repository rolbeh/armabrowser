using ArmaServerBrowser.Logic.DefaultImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmaServerBrowser.Logic
{
    internal static class LogicManager
    {
        public static ILogicContext CreateNewLogicContext()
        {
            return new LogicContext();
        }
    }
}
