using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UserFunc
{
    using lua_StatePtr = IntPtr;
    using size_t = Int32;

    class Luajit
    {
        [DllImport("luajit.dll", EntryPoint = "luaJIT_setmode",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public int luaJIT_setmode(lua_StatePtr L, int idx, int mode);

        [DllImport("luajit.dll", EntryPoint = "lua_CFunction",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        int lua_CFunction(lua_StatePtr L);


        [DllImport("luajit.dll", EntryPoint = "lua_Reader",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        char lua_Reader(lua_StatePtr L, IntPtr ud, IntPtr sz);

        [DllImport("luajit.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        int lua_Writer(lua_StatePtr L, IntPtr p, size_t sz, IntPtr ud);

        /*
            ** prototype for memory-allocation functions
        */
        [DllImport("luajit.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        lua_StatePtr lua_Alloc(IntPtr ud, IntPtr ptr, size_t osize, size_t nsize);

    }
}
