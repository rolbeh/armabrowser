using System;

namespace ArmaBrowser.Data.DefaultImpl
{
    [Flags]
    internal enum RulesStreamAttributes
    {
        None = 0x0,
        Hash = 0x1,
        PubId = 0x2,
        B1 = 0x4,
        B2 = 0x8
    }
}