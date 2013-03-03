using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LuaInterface
{
    using lua_StatePtr = IntPtr;
    using size_t = Int32;
    using size_t_p = IntPtr;
    using lua_Number = Double;
    using lua_Integer = Int32;


    public class LuaCommon
    {
        #region 静态方法及常数


        //public delegate int lua_CFunction(lua_StatePtr L);

        /*
        ** functions that read/write blocks when loading/dumping Lua chunks
        */
        public delegate string lua_Reader(lua_StatePtr L, IntPtr ud, size_t_p sz);

        public delegate int lua_Writer(lua_StatePtr L, IntPtr p, size_t sz, IntPtr ud);

        /*
            ** prototype for memory-allocation functions
        */
        public delegate IntPtr lua_Alloc(IntPtr ud, IntPtr ptr, size_t osize, size_t nsize);

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
** pseudo-indices
*/
        public const int LUA_REGISTRYINDEX = (-10000);
        public const int LUA_ENVIRONINDEX = (-10001);
        public const int LUA_GLOBALSINDEX = (-10002);
        public int lua_upvalueindex(int i) { return (LUA_GLOBALSINDEX - (i)); }


        /* thread status; 0 is OK */
        public const int LUA_YIELD = 1;
        public const int LUA_ERRRUN = 2;
        public const int LUA_ERRSYNTAX = 3;
        public const int LUA_ERRMEM = 4;
        public const int LUA_ERRERR = 5;

        /*
        ** state manipulation
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_StatePtr lua_newstate(lua_Alloc f, IntPtr ud);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_close(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_StatePtr lua_newthread(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern LuaCSFunction lua_atpanic(lua_StatePtr L, LuaCSFunction panicf);

        /*
        ** basic stack manipulation
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_gettop(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_settop(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushvalue(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_remove(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_insert(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_replace(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_checkstack(lua_StatePtr L, int sz);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_xmove(lua_StatePtr from, lua_StatePtr to, int n);

        /*
        ** access functions (stack -> C)
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_isnumber(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_isstring(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_iscfunction(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_isuserdata(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_type(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_typename(lua_StatePtr L, int tp);


        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_equal(lua_StatePtr L, int idx1, int idx2);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_rawequal(lua_StatePtr L, int idx1, int idx2);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_lessthan(lua_StatePtr L, int idx1, int idx2);


        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_Number lua_tonumber(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_Integer lua_tointeger(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_toboolean(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_tolstring(lua_StatePtr L, int idx, size_t_p len);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern size_t lua_objlen(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern LuaCSFunction lua_tocfunction(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr lua_touserdata(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_StatePtr lua_tothread(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr lua_topointer(lua_StatePtr L, int idx);


        /*
        ** push functions (C -> stack)
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushnil(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushnumber(lua_StatePtr L, lua_Number n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushinteger(lua_StatePtr L, lua_Integer n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushlstring(lua_StatePtr L, string s, size_t l);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushstring(lua_StatePtr L, string s);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_pushvfstring(lua_StatePtr L, string fmt,
                                                               params object[] argp);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern string lua_pushfstring(lua_StatePtr L, string fmt,
                                                               params object[] argp);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushcclosure(lua_StatePtr L, LuaCSFunction fn, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushboolean(lua_StatePtr L, int b);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_pushlightuserdata(lua_StatePtr L, IntPtr p);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_pushthread(lua_StatePtr L);


        /*
        ** get functions (Lua -> stack)
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_gettable(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_getfield(lua_StatePtr L, int idx, string k);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_rawget(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_rawgeti(lua_StatePtr L, int idx, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_createtable(lua_StatePtr L, int narr, int nrec);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr lua_newuserdata(lua_StatePtr L, size_t sz);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_getmetatable(lua_StatePtr L, int objindex);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_getfenv(lua_StatePtr L, int idx);


        /*
        ** set functions (stack -> Lua)
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_settable(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_setfield(lua_StatePtr L, int idx, string k);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_rawset(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_rawseti(lua_StatePtr L, int idx, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_setmetatable(lua_StatePtr L, int objindex);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_setfenv(lua_StatePtr L, int idx);


        /*
        ** `load' and `call' functions (load and run Lua code)
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_call(lua_StatePtr L, int nargs, int nresults);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_pcall(lua_StatePtr L, int nargs, int nresults, int errfunc);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_cpcall(lua_StatePtr L, LuaCSFunction func, IntPtr ud);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_load(lua_StatePtr L, lua_Reader reader, IntPtr dt,
                                               string chunkname);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_dump(lua_StatePtr L, lua_Writer writer, IntPtr data);


        /*
        ** coroutine functions
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_yield(lua_StatePtr L, int nresults);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_resume(lua_StatePtr L, int narg);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
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

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_gc(lua_StatePtr L, int what, int data);


        /*
        ** miscellaneous functions
        */
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_error(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_next(lua_StatePtr L, int idx);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_concat(lua_StatePtr L, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern lua_Alloc lua_getallocf(lua_StatePtr L, IntPtr ud);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern void lua_setallocf(lua_StatePtr L, lua_Alloc f, IntPtr ud);

        /*
        ** ===============================================================
        ** some useful macros
        ** ===============================================================
        */

        public static void lua_pop(lua_StatePtr L, size_t n) { lua_settop(L, -(n) - 1); }

        public static void lua_newtable(lua_StatePtr L) { lua_createtable(L, 0, 0); }

        public static void lua_register(lua_StatePtr L, string s, LuaCSFunction f)
        {
            lua_pushcfunction(L, (f));
            lua_setglobal(L, (s));
        }

        public static void lua_pushcfunction(lua_StatePtr L, LuaCSFunction f) { lua_pushcclosure(L, (f), 0); }

        public static lua_Integer lua_strlen(lua_StatePtr L, lua_Integer i) { return lua_objlen(L, (i)); }

        public static bool lua_isfunction(lua_StatePtr L, lua_Integer n) { return lua_type(L, (n)) == LUA_TFUNCTION; }
        public static bool lua_istable(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TTABLE); }
        public static bool lua_islightuserdata(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TLIGHTUSERDATA); }
        public static bool lua_isnil(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TNIL); }
        public static bool lua_isboolean(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TBOOLEAN); }
        public static bool lua_isthread(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TTHREAD); }
        public static bool lua_isnone(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) == LUA_TNONE); }
        public static bool lua_isnoneornil(lua_StatePtr L, lua_Integer n) { return (lua_type(L, (n)) <= 0); }

        public static void lua_pushliteral(lua_StatePtr L, string s)
        {
            lua_pushlstring(L, "" + s, (s.Length / sizeof(char)) - 1);
        }

        public static void lua_setglobal(lua_StatePtr L, string s) { lua_setfield(L, LUA_GLOBALSINDEX, (s)); }
        public static void lua_getglobal(lua_StatePtr L, string s) { lua_getfield(L, LUA_GLOBALSINDEX, (s)); }

        public static string lua_tostring(lua_StatePtr L, lua_Integer i) { return lua_tolstring(L, (i), IntPtr.Zero); }



        /*
        ** compatibility macros and functions
        */

        //void lua_open()	{ return luaL_newstate();}

        void lua_getregistry(lua_StatePtr L) { lua_pushvalue(L, LUA_REGISTRYINDEX); }

        lua_Integer lua_getgccount(lua_StatePtr L) { return lua_gc(L, LUA_GCCOUNT, 0); }

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_getstack(lua_StatePtr L, int level, IntPtr ar);
        
        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_getinfo(lua_StatePtr L, string what, IntPtr ar);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_getlocal(lua_StatePtr L, IntPtr ar, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_setlocal(lua_StatePtr L, IntPtr ar, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_getupvalue(lua_StatePtr L, int funcindex, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern string lua_setupvalue(lua_StatePtr L, int funcindex, int n);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_sethook(lua_StatePtr L, LuaHookFunction func, int mask, int count);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern LuaHookFunction lua_gethook(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_gethookmask(lua_StatePtr L);

        [DllImport("lua51.dll",
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        public static extern int lua_gethookcount(lua_StatePtr L);

        /* From Lua 5.2. */
        /*
        LUA_API void *lua_upvalueid (lua_State *L, int idx, int n);
        LUA_API void lua_upvaluejoin (lua_State *L, int idx1, int n1, int idx2, int n2);
        LUA_API int lua_loadx (lua_State *L, lua_Reader reader, void *dt,
               const char *chunkname, const char *mode);*/
        #endregion 静态方法及常数
    }
}
