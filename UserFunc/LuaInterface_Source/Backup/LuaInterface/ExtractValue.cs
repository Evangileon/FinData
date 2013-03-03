namespace LuaInterface
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate object ExtractValue(IntPtr luaState, int stackPos);
}

