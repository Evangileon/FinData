namespace LuaInterface
{
    using LuaInterface;
    using System;
    using System.Reflection;

    public class LuaUserData : LuaBase
    {
        public LuaUserData(int reference, Lua interpreter)
        {
            base._Reference = reference;
            base._Interpreter = interpreter;
        }

        public object[] Call(params object[] args)
        {
            return base._Interpreter.callFunction(this, args);
        }

        internal void push(IntPtr luaState)
        {
            LuaJIT.lua_getref(luaState, base._Reference);
        }

        public override string ToString()
        {
            return "userdata";
        }

        public object this[object field]
        {
            get
            {
                return base._Interpreter.getObject(base._Reference, field);
            }
            set
            {
                base._Interpreter.setObject(base._Reference, field, value);
            }
        }

        public object this[string field]
        {
            get
            {
                return base._Interpreter.getObject(base._Reference, field);
            }
            set
            {
                base._Interpreter.setObject(base._Reference, field, value);
            }
        }
    }
}

