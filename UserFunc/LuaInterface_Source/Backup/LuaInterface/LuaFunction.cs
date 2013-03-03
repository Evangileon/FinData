namespace LuaInterface
{
    using Lua511;
    using System;

    public class LuaFunction : LuaBase
    {
        internal LuaCSFunction function;

        public LuaFunction(LuaCSFunction function, Lua interpreter)
        {
            base._Reference = 0;
            this.function = function;
            base._Interpreter = interpreter;
        }

        public LuaFunction(int reference, Lua interpreter)
        {
            base._Reference = reference;
            this.function = null;
            base._Interpreter = interpreter;
        }

        internal object[] call(object[] args, Type[] returnTypes)
        {
            return base._Interpreter.callFunction(this, args, returnTypes);
        }

        public object[] Call(params object[] args)
        {
            return base._Interpreter.callFunction(this, args);
        }

        public override bool Equals(object o)
        {
            if (!(o is LuaFunction))
            {
                return false;
            }
            LuaFunction function = (LuaFunction) o;
            if ((base._Reference != 0) && (function._Reference != 0))
            {
                return base._Interpreter.compareRef(function._Reference, base._Reference);
            }
            return (this.function == function.function);
        }

        public override int GetHashCode()
        {
            if (base._Reference != 0)
            {
                return base._Reference;
            }
            return this.function.GetHashCode();
        }

        internal void push(IntPtr luaState)
        {
            if (base._Reference != 0)
            {
                LuaDLL.lua_getref(luaState, base._Reference);
            }
            else
            {
                base._Interpreter.pushCSFunction(this.function);
            }
        }

        public override string ToString()
        {
            return "function";
        }
    }
}

