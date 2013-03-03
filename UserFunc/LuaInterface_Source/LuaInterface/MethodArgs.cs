namespace LuaInterface
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MethodArgs
    {
        public int index;
        public ExtractValue extractValue;
    }
}

