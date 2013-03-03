using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using LuaInterface;

namespace LuaInterface
{
    using lua_StatePtr = IntPtr;
    using size_t = Int32;
    using size_t_p = IntPtr;
    using lua_Number = Double;
    using lua_Integer = Int32;

    enum Luajit_mode
    {
        LUAJIT_MODE_ENGINE,		/* Set mode for whole JIT engine. */
        LUAJIT_MODE_DEBUG,		/* Set debug mode (idx = level). */

        LUAJIT_MODE_FUNC,		/* Change mode for a function. */
        LUAJIT_MODE_ALLFUNC,		/* Recurse into subroutine protos. */
        LUAJIT_MODE_ALLSUBFUNC,	/* Change only the subroutines. */

        LUAJIT_MODE_TRACE,		/* Flush a compiled trace. */

        LUAJIT_MODE_WRAPCFUNC = 0x10,	/* Set wrapper mode for C function calls. */

        LUAJIT_MODE_MAX
    }


    public class LuaJIT : LuaCommon
    {
        //lua_StatePtr instance;
        //size_t_p ud;
        private Lua lua_instance;

        public LuaJIT()
        {
            //this.instance = new lua_StatePtr();
            //this.ud = new size_t_p(0);
            //this.instance = Luajit.lua_newstate(normal_lua_Alloc, ud);
            this.lua_instance = new Lua();
            //lua_instance.
        }

        public const int LUAJIT_MODE_OFF = 0x0000;	/* Turn feature off. */
        public const int LUAJIT_MODE_ON = 0x0100;	/* Turn feature on. */
        public const int LUAJIT_MODE_FLUSH = 0x0200;	/* Flush JIT-compiled code. */

        [DllImport("lua51.dll", EntryPoint = "luaJIT_setmode",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int luaJIT_setmode(lua_StatePtr L, int idx, int mode);
    }
}
