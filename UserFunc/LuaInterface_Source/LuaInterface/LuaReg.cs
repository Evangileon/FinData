using System;
using System.Collections.Generic;
using System.Text;

namespace LuaInterface
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct luaL_Reg
    {
        [MarshalAs(UnmanagedType.LPStr)]
        string name;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        LuaCSFunction func;
    }   
}
