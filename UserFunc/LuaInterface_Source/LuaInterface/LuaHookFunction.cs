namespace LuaInterface
{
    using System;
    using System.Runtime.CompilerServices;

    public delegate void LuaHookFunction(IntPtr luaState, IntPtr luaDebug);
}

