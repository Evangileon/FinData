using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UserFunc
{
    using lua_StatePtr = IntPtr;
    using size_t = Int32;
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


    class Luajit
    {
        public const int LUAJIT_MODE_OFF = 0x0000;	/* Turn feature off. */
        public const int LUAJIT_MODE_ON = 0x0100;	/* Turn feature on. */
        public const int LUAJIT_MODE_FLUSH = 0x0200;	/* Flush JIT-compiled code. */

        [DllImport("lua51.dll", EntryPoint = "luaJIT_setmode",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int luaJIT_setmode(lua_StatePtr L, int idx, int mode);

        delegate int lua_CFunction(lua_StatePtr L);

        /*
        ** functions that read/write blocks when loading/dumping Lua chunks
        */
        delegate string lua_Reader(lua_StatePtr L, IntPtr ud, IntPtr sz);

        delegate int lua_Writer(lua_StatePtr L, IntPtr p, size_t sz, IntPtr ud);

        /*
            ** prototype for memory-allocation functions
        */
        delegate IntPtr lua_Alloc(IntPtr ud, IntPtr ptr, size_t osize, size_t nsize);

        /*
        ** basic types
        */
        public const int LUA_TNONE = (-1);
        public const int LUA_TNIL = 0;
        public const int LUA_TBOOLEAN = 1;
        public const int LUA_TLIGHTUSERDATA = 2;
        public const int LUA_TNUMBER = 3;
        public const int LUA_TSTRING = 4;
        public const int LUA_TTABLE = 5;
        public const int LUA_TFUNCTION = 6;
        public const int LUA_TUSERDATA = 7;
        public const int LUA_TTHREAD = 8;
        /* minimum Lua stack available to a C function */
        public const int LUA_MINSTACK = 20;

        /*
        ** state manipulation
        */
        public static extern lua_StatePtr lua_newstate(lua_Alloc f, IntPtr ud);
        public static extern void lua_close(lua_StatePtr L);
        public static extern lua_StatePtr lua_newthread(lua_StatePtr L);

        public static extern lua_CFunction lua_atpanic(lua_StatePtr L, lua_CFunction panicf);

        /*
        ** basic stack manipulation
        */
        public static extern int lua_gettop(lua_StatePtr L);
        public static extern void lua_settop(lua_StatePtr L, int idx);
        public static extern void lua_pushvalue(lua_StatePtr L, int idx);
        public static extern void lua_remove(lua_StatePtr L, int idx);
        public static extern void lua_insert(lua_StatePtr L, int idx);
        public static extern void lua_replace(lua_StatePtr L, int idx);
        public static extern int lua_checkstack(lua_StatePtr L, int sz);

        public static extern void lua_xmove(lua_StatePtr from, lua_StatePtr to, int n);

        /*
        ** access functions (stack -> C)
        */
        public static extern int lua_isnumber(lua_StatePtr L, int idx);
        public static extern int lua_isstring(lua_StatePtr L, int idx);
        public static extern int lua_iscfunction(lua_StatePtr L, int idx);
        public static extern int lua_isuserdata(lua_StatePtr L, int idx);
        public static extern int lua_type(lua_StatePtr L, int idx);
        public static extern string lua_typename(lua_StatePtr L, int tp);

        public static extern int lua_equal(lua_StatePtr L, int idx1, int idx2);
        public static extern int lua_rawequal(lua_StatePtr L, int idx1, int idx2);
        public static extern int lua_lessthan(lua_StatePtr L, int idx1, int idx2);

        public static extern lua_Number lua_tonumber(lua_StatePtr L, int idx);
        public static extern lua_Integer lua_tointeger(lua_StatePtr L, int idx);
        public static extern int lua_toboolean(lua_StatePtr L, int idx);
        public static extern string lua_tolstring(lua_StatePtr L, int idx, size_t* len);
        public static extern size_t lua_objlen(lua_StatePtr L, int idx);
        public static extern lua_CFunction lua_tocfunction(lua_StatePtr L, int idx);
        public static extern IntPtr lua_touserdata(lua_StatePtr L, int idx);
        public static extern lua_StatePtr lua_tothread(lua_StatePtr L, int idx);
        public static extern IntPtr lua_topointer(lua_StatePtr L, int idx);


        /*
        ** push functions (C -> stack)
        */
        public static extern void lua_pushnil(lua_StatePtr L);
        public static extern void lua_pushnumber(lua_StatePtr L, lua_Number n);
        public static extern void lua_pushinteger(lua_StatePtr L, lua_Integer n);
        public static extern void lua_pushlstring(lua_StatePtr L, string s, size_t l);
        public static extern void lua_pushstring(lua_StatePtr L, string s);
        public static extern string lua_pushvfstring(lua_StatePtr L, string fmt,
                                                               params object[] argp);
        public static extern string lua_pushfstring(lua_StatePtr L, string fmt,
                                                               params object[] argp);
        public static extern void lua_pushcclosure(lua_StatePtr L, lua_CFunction fn, int n);
        public static extern void lua_pushboolean(lua_StatePtr L, int b);
        public static extern void lua_pushlightuserdata(lua_StatePtr L, IntPtr p);
        public static extern int lua_pushthread(lua_StatePtr L);


        /*
        ** get functions (Lua -> stack)
        */
        public static extern void lua_gettable(lua_StatePtr L, int idx);
        public static extern void lua_getfield(lua_StatePtr L, int idx, string k);
        public static extern void lua_rawget(lua_StatePtr L, int idx);
        public static extern void lua_rawgeti(lua_StatePtr L, int idx, int n);
        public static extern void lua_createtable(lua_StatePtr L, int narr, int nrec);
        public static extern IntPtr lua_newuserdata(lua_StatePtr L, size_t sz);
        public static extern int lua_getmetatable(lua_StatePtr L, int objindex);
        public static extern void lua_getfenv(lua_StatePtr L, int idx);


        /*
        ** set functions (stack -> Lua)
        */
        public static extern void lua_settable(lua_StatePtr L, int idx);
        public static extern void lua_setfield(lua_StatePtr L, int idx, string k);
        public static extern void lua_rawset(lua_StatePtr L, int idx);
        public static extern void lua_rawseti(lua_StatePtr L, int idx, int n);
        public static extern int lua_setmetatable(lua_StatePtr L, int objindex);
        public static extern int lua_setfenv(lua_StatePtr L, int idx);


        /*
        ** `load' and `call' functions (load and run Lua code)
        */
        public static extern void lua_call(lua_StatePtr L, int nargs, int nresults);
        public static extern int lua_pcall(lua_StatePtr L, int nargs, int nresults, int errfunc);
        public static extern int lua_cpcall(lua_StatePtr L, lua_CFunction func, IntPtr ud);
        public static extern int lua_load(lua_StatePtr L, lua_Reader reader, IntPtr dt,
                                               string chunkname);
        public static extern int lua_dump(lua_StatePtr L, lua_Writer writer, IntPtr data);


        /*
        ** coroutine functions
        */
        public static extern int lua_yield(lua_StatePtr L, int nresults);
        public static extern int lua_resume(lua_StatePtr L, int narg);
        public static extern int lua_status(lua_StatePtr L);

        /*
        ** garbage-collection function and options
        */
        public const int LUA_GCSTOP = 0;
        public const int LUA_GCRESTART = 1;
        public const int LUA_GCCOLLECT = 2;
        public const int LUA_GCCOUNT = 3;
        public const int LUA_GCCOUNTB = 4;
        public const int LUA_GCSTEP = 5;
        public const int LUA_GCSETPAUSE = 6;
        public const int LUA_GCSETSTEPMUL = 7;

        public static extern int lua_gc(lua_StatePtr L, int what, int data);


        /*
        ** miscellaneous functions
        */
        public static extern int lua_error(lua_StatePtr L);

        public static extern int lua_next(lua_StatePtr L, int idx);

        public static extern void lua_concat(lua_StatePtr L, int n);

        public static extern lua_Alloc lua_getallocf(lua_StatePtr L, IntPtr* ud);
        public static extern void lua_setallocf(lua_StatePtr L, lua_Alloc f, IntPtr ud);
    }
}
