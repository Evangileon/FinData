namespace LuaInterface
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct LuaDebug
    {
        public EventCodes eventCode;
        [MarshalAs(UnmanagedType.LPStr)]
        public string name;
        [MarshalAs(UnmanagedType.LPStr)]
        public string namewhat;
        [MarshalAs(UnmanagedType.LPStr)]
        public string what;
        [MarshalAs(UnmanagedType.LPStr)]
        public string source;
        public int currentline;
        public int nups;
        public int linedefined;
        public int lastlinedefined;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=60)]
        public string shortsrc;
        public int i_ci;
    }
}

