namespace LuaInterface
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MethodCache
    {
        public MethodBase cachedMethod;
        public object[] args;
        public int[] outList;
        public MethodArgs[] argTypes;
    }
}

