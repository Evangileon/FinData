namespace LuaInterface
{
    using System;

    public class DebugHookEventArgs : EventArgs
    {
        private readonly LuaInterface.LuaDebug luaDebug;

        public DebugHookEventArgs(LuaInterface.LuaDebug luaDebug)
        {
            this.luaDebug = luaDebug;
        }

        public LuaInterface.LuaDebug LuaDebug
        {
            get
            {
                return this.luaDebug;
            }
        }
    }
}

