namespace LuaInterface
{
    using Lua511;
    using System;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class Lua : IDisposable
    {
        private bool _StatePassed;
        private LuaHookFunction hookCallback;
        private static string init_luanet = "local metatable = {}\t\t\t\t\t\t\t\t\t\nlocal import_type = luanet.import_type\t\t\t\t\t\t\t\nlocal load_assembly = luanet.load_assembly\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n-- Lookup a .NET identifier component.\t\t\t\t\t\t\t\nfunction metatable:__index(key) -- key is e.g. \"Form\"\t\t\t\t\n    -- Get the fully-qualified name, e.g. \"System.Windows.Forms.Form\"\t\t\n    local fqn = ((rawget(self,\".fqn\") and rawget(self,\".fqn\") ..\t\t\t\n\t\t\".\") or \"\") .. key\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n    -- Try to find either a luanet function or a CLR type\t\t\t\t\n    local obj = rawget(luanet,key) or import_type(fqn)\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n    -- If key is neither a luanet function or a CLR type, then it is simply\t\t\n    -- an identifier component.\t\t\t\t\t\t\t\n    if obj == nil then\t\t\t\t\t\t\t\t\t\n\t\t-- It might be an assembly, so we load it too.\t\t\t\t\n        load_assembly(fqn)\t\t\t\t\t\t\t\t\n        obj = { [\".fqn\"] = fqn }\t\t\t\t\t\t\t\n        setmetatable(obj, metatable)\t\t\t\t\t\t\t\n    end\t\t\t\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n    -- Cache this lookup\t\t\t\t\t\t\t\t\n    rawset(self, key, obj)\t\t\t\t\t\t\t\t\n    return obj\t\t\t\t\t\t\t\t\t\t\nend\t\t\t\t\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n-- A non-type has been called; e.g. foo = System.Foo()\t\t\t\t\t\nfunction metatable:__call(...)\t\t\t\t\t\t\t\t\n    error(\"No such type: \" .. rawget(self,\".fqn\"), 2)\t\t\t\t\nend\t\t\t\t\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n-- This is the root of the .NET namespace\t\t\t\t\t\t\nluanet[\".fqn\"] = false\t\t\t\t\t\t\t\t\nsetmetatable(luanet, metatable)\t\t\t\t\t\t\t\n\t\t\t\t\t\t\t\t\t\t\t\n-- Preload the mscorlib assembly\t\t\t\t\t\t\t\nluanet.load_assembly(\"mscorlib\")\t\t\t\t\t\t\t\n";
        private LuaCSFunction lockCallback;
        private object luaLock;
        private IntPtr luaState;
        private LuaCSFunction panicCallback;
        private ObjectTranslator translator;
        private LuaCSFunction unlockCallback;

        public event EventHandler<DebugHookEventArgs> DebugHook;

        public event EventHandler<HookExceptionEventArgs> HookException;

        public Lua()
        {
            this.luaLock = new object();
            this.luaState = LuaDLL.luaL_newstate();
            LuaDLL.luaL_openlibs(this.luaState);
            LuaDLL.lua_pushstring(this.luaState, "LUAINTERFACE LOADED");
            LuaDLL.lua_pushboolean(this.luaState, true);
            LuaDLL.lua_settable(this.luaState, -10000);
            LuaDLL.lua_newtable(this.luaState);
            LuaDLL.lua_setglobal(this.luaState, "luanet");
            LuaDLL.lua_pushvalue(this.luaState, -10002);
            LuaDLL.lua_getglobal(this.luaState, "luanet");
            LuaDLL.lua_pushstring(this.luaState, "getmetatable");
            LuaDLL.lua_getglobal(this.luaState, "getmetatable");
            LuaDLL.lua_settable(this.luaState, -3);
            LuaDLL.lua_replace(this.luaState, -10002);
            this.translator = new ObjectTranslator(this, this.luaState);
            LuaDLL.lua_replace(this.luaState, -10002);
            LuaDLL.luaL_dostring(this.luaState, init_luanet);
            this.panicCallback = new LuaCSFunction(Lua.PanicCallback);
            LuaDLL.lua_atpanic(this.luaState, this.panicCallback);
        }

        public Lua(long luaState)
        {
            this.luaLock = new object();
            IntPtr ptr = new IntPtr(luaState);
            LuaDLL.lua_pushstring(ptr, "LUAINTERFACE LOADED");
            LuaDLL.lua_gettable(ptr, -10000);
            if (LuaDLL.lua_toboolean(ptr, -1))
            {
                LuaDLL.lua_settop(ptr, -2);
                throw new LuaException("There is already a LuaInterface.Lua instance associated with this Lua state");
            }
            LuaDLL.lua_settop(ptr, -2);
            LuaDLL.lua_pushstring(ptr, "LUAINTERFACE LOADED");
            LuaDLL.lua_pushboolean(ptr, true);
            LuaDLL.lua_settable(ptr, -10000);
            this.luaState = ptr;
            LuaDLL.lua_pushvalue(ptr, -10002);
            LuaDLL.lua_getglobal(ptr, "luanet");
            LuaDLL.lua_pushstring(ptr, "getmetatable");
            LuaDLL.lua_getglobal(ptr, "getmetatable");
            LuaDLL.lua_settable(ptr, -3);
            LuaDLL.lua_replace(ptr, -10002);
            this.translator = new ObjectTranslator(this, this.luaState);
            LuaDLL.lua_replace(ptr, -10002);
            LuaDLL.luaL_dostring(ptr, init_luanet);
            this._StatePassed = true;
        }

        internal object[] callFunction(object function, object[] args)
        {
            return this.callFunction(function, args, null);
        }

        internal object[] callFunction(object function, object[] args, Type[] returnTypes)
        {
            int nArgs = 0;
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (!LuaDLL.lua_checkstack(this.luaState, args.Length + 6))
            {
                throw new LuaException("Lua stack overflow");
            }
            this.translator.push(this.luaState, function);
            if (args != null)
            {
                nArgs = args.Length;
                for (int i = 0; i < args.Length; i++)
                {
                    this.translator.push(this.luaState, args[i]);
                }
            }
            if (LuaDLL.lua_pcall(this.luaState, nArgs, -1, 0) != 0)
            {
                this.ThrowExceptionFromError(oldTop);
            }
            if (returnTypes != null)
            {
                return this.translator.popValues(this.luaState, oldTop, returnTypes);
            }
            return this.translator.popValues(this.luaState, oldTop);
        }

        public void Close()
        {
            if (!this._StatePassed && (this.luaState != IntPtr.Zero))
            {
                LuaDLL.lua_close(this.luaState);
            }
        }

        internal bool compareRef(int ref1, int ref2)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, ref1);
            LuaDLL.lua_getref(this.luaState, ref2);
            int num2 = LuaDLL.lua_equal(this.luaState, -1, -2);
            LuaDLL.lua_settop(this.luaState, newTop);
            return (num2 != 0);
        }

        private void DebugHookCallback(IntPtr luaState, IntPtr luaDebug)
        {
            try
            {
                LuaDebug debug = (LuaDebug) Marshal.PtrToStructure(luaDebug, typeof(LuaDebug));
                EventHandler<DebugHookEventArgs> debugHook = this.DebugHook;
                if (debugHook != null)
                {
                    debugHook(this, new DebugHookEventArgs(debug));
                }
            }
            catch (Exception exception)
            {
                this.OnHookException(new HookExceptionEventArgs(exception));
            }
        }

        internal void dispose(int reference)
        {
            if (this.luaState != IntPtr.Zero)
            {
                LuaDLL.lua_unref(this.luaState, reference);
            }
        }

        public virtual void Dispose()
        {
            if (this.translator != null)
            {
                this.translator.pendingEvents.Dispose();
                this.translator = null;
            }
            this.Close();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public object[] DoFile(string fileName)
        {
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (LuaDLL.luaL_loadfile(this.luaState, fileName) == 0)
            {
                if (LuaDLL.lua_pcall(this.luaState, 0, -1, 0) == 0)
                {
                    return this.translator.popValues(this.luaState, oldTop);
                }
                this.ThrowExceptionFromError(oldTop);
            }
            else
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return null;
        }

        public object[] DoString(string chunk)
        {
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (LuaDLL.luaL_loadbuffer(this.luaState, chunk, "chunk") == 0)
            {
                if (LuaDLL.lua_pcall(this.luaState, 0, -1, 0) == 0)
                {
                    return this.translator.popValues(this.luaState, oldTop);
                }
                this.ThrowExceptionFromError(oldTop);
            }
            else
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return null;
        }

        public object[] DoString(string chunk, string chunkName)
        {
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (LuaDLL.luaL_loadbuffer(this.luaState, chunk, chunkName) == 0)
            {
                if (LuaDLL.lua_pcall(this.luaState, 0, -1, 0) == 0)
                {
                    return this.translator.popValues(this.luaState, oldTop);
                }
                this.ThrowExceptionFromError(oldTop);
            }
            else
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return null;
        }

        public LuaFunction GetFunction(string fullPath)
        {
            object obj2 = this[fullPath];
            if (obj2 is LuaCSFunction)
            {
                return new LuaFunction((LuaCSFunction) obj2, this);
            }
            return (LuaFunction) obj2;
        }

        public Delegate GetFunction(Type delegateType, string fullPath)
        {
            return CodeGeneration.Instance.GetDelegate(delegateType, this.GetFunction(fullPath));
        }

        public int GetHookCount()
        {
            return LuaDLL.lua_gethookcount(this.luaState);
        }

        public EventMasks GetHookMask()
        {
            return (EventMasks) LuaDLL.lua_gethookmask(this.luaState);
        }

        public int GetInfo(string what, ref LuaDebug luaDebug)
        {
            int num;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf((LuaDebug) luaDebug));
            Marshal.StructureToPtr((LuaDebug) luaDebug, ptr, false);
            try
            {
                num = LuaDLL.lua_getinfo(this.luaState, what, ptr);
            }
            finally
            {
                luaDebug = (LuaDebug) Marshal.PtrToStructure(ptr, typeof(LuaDebug));
                Marshal.FreeHGlobal(ptr);
            }
            return num;
        }

        public string GetLocal(LuaDebug luaDebug, int n)
        {
            string str;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(luaDebug));
            Marshal.StructureToPtr(luaDebug, ptr, false);
            try
            {
                str = LuaDLL.lua_getlocal(this.luaState, ptr, n);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return str;
        }

        public double GetNumber(string fullPath)
        {
            return (double) this[fullPath];
        }

        internal object getObject(string[] remainingPath)
        {
            object obj2 = null;
            for (int i = 0; i < remainingPath.Length; i++)
            {
                LuaDLL.lua_pushstring(this.luaState, remainingPath[i]);
                LuaDLL.lua_gettable(this.luaState, -2);
                obj2 = this.translator.getObject(this.luaState, -1);
                if (obj2 == null)
                {
                    return obj2;
                }
            }
            return obj2;
        }

        internal object getObject(int reference, object field)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, reference);
            this.translator.push(this.luaState, field);
            LuaDLL.lua_gettable(this.luaState, -2);
            object obj2 = this.translator.getObject(this.luaState, -1);
            LuaDLL.lua_settop(this.luaState, newTop);
            return obj2;
        }

        internal object getObject(int reference, string field)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, reference);
            object obj2 = this.getObject(field.Split(new char[] { '.' }));
            LuaDLL.lua_settop(this.luaState, newTop);
            return obj2;
        }

        public bool GetStack(int level, out LuaDebug luaDebug)
        {
            bool flag;
            luaDebug = new LuaDebug();
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf((LuaDebug) luaDebug));
            Marshal.StructureToPtr((LuaDebug) luaDebug, ptr, false);
            try
            {
                flag = LuaDLL.lua_getstack(this.luaState, level, ptr) != 0;
            }
            finally
            {
                luaDebug = (LuaDebug) Marshal.PtrToStructure(ptr, typeof(LuaDebug));
                Marshal.FreeHGlobal(ptr);
            }
            return flag;
        }

        public string GetString(string fullPath)
        {
            return (string) this[fullPath];
        }

        public LuaTable GetTable(string fullPath)
        {
            return (LuaTable) this[fullPath];
        }

        public object GetTable(Type interfaceType, string fullPath)
        {
            return CodeGeneration.Instance.GetClassInstance(interfaceType, this.GetTable(fullPath));
        }

        public ListDictionary GetTableDict(LuaTable table)
        {
            ListDictionary dictionary = new ListDictionary();
            int newTop = LuaDLL.lua_gettop(this.luaState);
            this.translator.push(this.luaState, table);
            LuaDLL.lua_pushnil(this.luaState);
            while (LuaDLL.lua_next(this.luaState, -2) != 0)
            {
                dictionary[this.translator.getObject(this.luaState, -2)] = this.translator.getObject(this.luaState, -1);
                LuaDLL.lua_settop(this.luaState, -2);
            }
            LuaDLL.lua_settop(this.luaState, newTop);
            return dictionary;
        }

        public string GetUpValue(int funcindex, int n)
        {
            return LuaDLL.lua_getupvalue(this.luaState, funcindex, n);
        }

        public LuaFunction LoadFile(string fileName)
        {
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (LuaDLL.luaL_loadfile(this.luaState, fileName) != 0)
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return this.translator.getFunction(this.luaState, -1);
        }

        public LuaFunction LoadString(string chunk, string name)
        {
            int oldTop = LuaDLL.lua_gettop(this.luaState);
            if (LuaDLL.luaL_loadbuffer(this.luaState, chunk, name) != 0)
            {
                this.ThrowExceptionFromError(oldTop);
            }
            return this.translator.getFunction(this.luaState, -1);
        }

        private int LockCallback(IntPtr luaState)
        {
            return 0;
        }

        public void NewTable(string fullPath)
        {
            string[] strArray = fullPath.Split(new char[] { '.' });
            int newTop = LuaDLL.lua_gettop(this.luaState);
            if (strArray.Length == 1)
            {
                LuaDLL.lua_newtable(this.luaState);
                LuaDLL.lua_setglobal(this.luaState, fullPath);
            }
            else
            {
                LuaDLL.lua_getglobal(this.luaState, strArray[0]);
                for (int i = 1; i < (strArray.Length - 1); i++)
                {
                    LuaDLL.lua_pushstring(this.luaState, strArray[i]);
                    LuaDLL.lua_gettable(this.luaState, -2);
                }
                LuaDLL.lua_pushstring(this.luaState, strArray[strArray.Length - 1]);
                LuaDLL.lua_newtable(this.luaState);
                LuaDLL.lua_settable(this.luaState, -3);
            }
            LuaDLL.lua_settop(this.luaState, newTop);
        }

        private void OnHookException(HookExceptionEventArgs e)
        {
            EventHandler<HookExceptionEventArgs> hookException = this.HookException;
            if (hookException != null)
            {
                hookException(this, e);
            }
        }

        private static int PanicCallback(IntPtr luaState)
        {
            throw new LuaException(string.Format("unprotected error in call to Lua API ({0})", LuaDLL.lua_tostring(luaState, -1)));
        }

        public object Pop()
        {
            int num = LuaDLL.lua_gettop(this.luaState);
            return this.translator.popValues(this.luaState, num - 1)[0];
        }

        public void Push(object value)
        {
            this.translator.push(this.luaState, value);
        }

        internal void pushCSFunction(LuaCSFunction function)
        {
            this.translator.pushFunction(this.luaState, function);
        }

        internal object rawGetObject(int reference, string field)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, reference);
            LuaDLL.lua_pushstring(this.luaState, field);
            LuaDLL.lua_rawget(this.luaState, -2);
            object obj2 = this.translator.getObject(this.luaState, -1);
            LuaDLL.lua_settop(this.luaState, newTop);
            return obj2;
        }

        public LuaFunction RegisterFunction(string path, object target, MethodBase function)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaMethodWrapper wrapper = new LuaMethodWrapper(this.translator, target, function.DeclaringType, function);
            this.translator.push(this.luaState, new LuaCSFunction(wrapper.call));
            this[path] = this.translator.getObject(this.luaState, -1);
            LuaFunction function2 = this.GetFunction(path);
            LuaDLL.lua_settop(this.luaState, newTop);
            return function2;
        }

        public int RemoveDebugHook()
        {
            this.hookCallback = null;
            return LuaDLL.lua_sethook(this.luaState, null, 0, 0);
        }

        public int SetDebugHook(EventMasks mask, int count)
        {
            if (this.hookCallback == null)
            {
                this.hookCallback = new LuaHookFunction(this.DebugHookCallback);
                return LuaDLL.lua_sethook(this.luaState, this.hookCallback, (int) mask, count);
            }
            return -1;
        }

        public string SetLocal(LuaDebug luaDebug, int n)
        {
            string str;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(luaDebug));
            Marshal.StructureToPtr(luaDebug, ptr, false);
            try
            {
                str = LuaDLL.lua_setlocal(this.luaState, ptr, n);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return str;
        }

        internal void setObject(string[] remainingPath, object val)
        {
            for (int i = 0; i < (remainingPath.Length - 1); i++)
            {
                LuaDLL.lua_pushstring(this.luaState, remainingPath[i]);
                LuaDLL.lua_gettable(this.luaState, -2);
            }
            LuaDLL.lua_pushstring(this.luaState, remainingPath[remainingPath.Length - 1]);
            this.translator.push(this.luaState, val);
            LuaDLL.lua_settable(this.luaState, -3);
        }

        internal void setObject(int reference, object field, object val)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, reference);
            this.translator.push(this.luaState, field);
            this.translator.push(this.luaState, val);
            LuaDLL.lua_settable(this.luaState, -3);
            LuaDLL.lua_settop(this.luaState, newTop);
        }

        internal void setObject(int reference, string field, object val)
        {
            int newTop = LuaDLL.lua_gettop(this.luaState);
            LuaDLL.lua_getref(this.luaState, reference);
            this.setObject(field.Split(new char[] { '.' }), val);
            LuaDLL.lua_settop(this.luaState, newTop);
        }

        internal int SetPendingException(Exception e)
        {
            Exception exception = e;
            if (exception != null)
            {
                this.translator.throwError(this.luaState, exception);
                LuaDLL.lua_pushnil(this.luaState);
                return 1;
            }
            return 0;
        }

        public string SetUpValue(int funcindex, int n)
        {
            return LuaDLL.lua_setupvalue(this.luaState, funcindex, n);
        }

        private void ThrowExceptionFromError(int oldTop)
        {
            object obj2 = this.translator.getObject(this.luaState, -1);
            LuaDLL.lua_settop(this.luaState, oldTop);
            Exception exception = obj2 as Exception;
            if (exception == null)
            {
                if (obj2 == null)
                {
                    obj2 = "Unknown Lua Error";
                }
                exception = new LuaException(obj2.ToString());
            }
            throw exception;
        }

        private int UnlockCallback(IntPtr luaState)
        {
            return 0;
        }

        public object this[string fullPath]
        {
            get
            {
                object obj2 = null;
                int newTop = LuaDLL.lua_gettop(this.luaState);
                string[] sourceArray = fullPath.Split(new char[] { '.' });
                LuaDLL.lua_getglobal(this.luaState, sourceArray[0]);
                obj2 = this.translator.getObject(this.luaState, -1);
                if (sourceArray.Length > 1)
                {
                    string[] destinationArray = new string[sourceArray.Length - 1];
                    Array.Copy(sourceArray, 1, destinationArray, 0, sourceArray.Length - 1);
                    obj2 = this.getObject(destinationArray);
                }
                LuaDLL.lua_settop(this.luaState, newTop);
                return obj2;
            }
            set
            {
                int newTop = LuaDLL.lua_gettop(this.luaState);
                string[] sourceArray = fullPath.Split(new char[] { '.' });
                if (sourceArray.Length == 1)
                {
                    this.translator.push(this.luaState, value);
                    LuaDLL.lua_setglobal(this.luaState, fullPath);
                }
                else
                {
                    LuaDLL.lua_getglobal(this.luaState, sourceArray[0]);
                    string[] destinationArray = new string[sourceArray.Length - 1];
                    Array.Copy(sourceArray, 1, destinationArray, 0, sourceArray.Length - 1);
                    this.setObject(destinationArray, value);
                }
                LuaDLL.lua_settop(this.luaState, newTop);
            }
        }
    }
}

