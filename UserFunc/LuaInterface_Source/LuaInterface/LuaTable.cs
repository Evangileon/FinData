namespace LuaInterface
{
    using LuaInterface;
    using System;
    using System.Collections;
    using System.Reflection;

    public class LuaTable : LuaBase
    {
        public LuaTable(int reference, Lua interpreter)
        {
            base._Reference = reference;
            base._Interpreter = interpreter;
        }

        public IEnumerator GetEnumerator()
        {
            return base._Interpreter.GetTableDict(this).GetEnumerator();
        }

        internal void push(IntPtr luaState)
        {
            LuaJIT.lua_getref(luaState, base._Reference);
        }

        internal object rawget(string field)
        {
            return base._Interpreter.rawGetObject(base._Reference, field);
        }

        internal object rawgetFunction(string field)
        {
            object obj2 = base._Interpreter.rawGetObject(base._Reference, field);
            if (obj2 is LuaCSFunction)
            {
                return new LuaFunction((LuaCSFunction) obj2, base._Interpreter);
            }
            return obj2;
        }

        public override string ToString()
        {
            return "table";
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

        public ICollection Keys
        {
            get
            {
                return base._Interpreter.GetTableDict(this).Keys;
            }
        }

        public ICollection Values
        {
            get
            {
                return base._Interpreter.GetTableDict(this).Values;
            }
        }
    }
}

