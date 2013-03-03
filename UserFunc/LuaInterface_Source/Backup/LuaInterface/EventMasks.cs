namespace LuaInterface
{
    using System;

    [Flags]
    public enum EventMasks
    {
        LUA_MASKALL = 0x7fffffff,
        LUA_MASKCALL = 1,
        LUA_MASKCOUNT = 8,
        LUA_MASKLINE = 4,
        LUA_MASKRET = 2
    }
}

