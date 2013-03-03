namespace LuaInterface
{
    using Lua511;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class CheckType
    {
        private ExtractValue extractNetObject;
        private Dictionary<long, ExtractValue> extractValues = new Dictionary<long, ExtractValue>();
        private ObjectTranslator translator;

        public CheckType(ObjectTranslator translator)
        {
            this.translator = translator;
            this.extractValues.Add(typeof(object).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsObject));
            this.extractValues.Add(typeof(sbyte).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsSbyte));
            this.extractValues.Add(typeof(byte).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsByte));
            this.extractValues.Add(typeof(short).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsShort));
            this.extractValues.Add(typeof(ushort).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUshort));
            this.extractValues.Add(typeof(int).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsInt));
            this.extractValues.Add(typeof(uint).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUint));
            this.extractValues.Add(typeof(long).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsLong));
            this.extractValues.Add(typeof(ulong).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUlong));
            this.extractValues.Add(typeof(double).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsDouble));
            this.extractValues.Add(typeof(char).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsChar));
            this.extractValues.Add(typeof(float).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsFloat));
            this.extractValues.Add(typeof(decimal).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsDecimal));
            this.extractValues.Add(typeof(bool).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsBoolean));
            this.extractValues.Add(typeof(string).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsString));
            this.extractValues.Add(typeof(LuaFunction).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsFunction));
            this.extractValues.Add(typeof(LuaTable).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsTable));
            this.extractValues.Add(typeof(LuaUserData).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUserdata));
            this.extractNetObject = new ExtractValue(this.getAsNetObject);
        }

        internal ExtractValue checkType(IntPtr luaState, int stackPos, Type paramType)
        {
            LuaTypes types = LuaDLL.lua_type(luaState, stackPos);
            if (paramType.IsByRef)
            {
                paramType = paramType.GetElementType();
            }
            Type underlyingType = Nullable.GetUnderlyingType(paramType);
            if (underlyingType != null)
            {
                paramType = underlyingType;
            }
            long num = paramType.TypeHandle.Value.ToInt64();
            if (paramType.Equals(typeof(object)))
            {
                return this.extractValues[num];
            }
            if (LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return this.extractValues[num];
            }
            if (paramType == typeof(bool))
            {
                if (LuaDLL.lua_isboolean(luaState, stackPos))
                {
                    return this.extractValues[num];
                }
            }
            else if (paramType == typeof(string))
            {
                if (LuaDLL.lua_isstring(luaState, stackPos))
                {
                    return this.extractValues[num];
                }
                if (types == LuaTypes.LUA_TNIL)
                {
                    return this.extractNetObject;
                }
            }
            else if (paramType == typeof(LuaTable))
            {
                if (types == LuaTypes.LUA_TTABLE)
                {
                    return this.extractValues[num];
                }
            }
            else if (paramType == typeof(LuaUserData))
            {
                if (types == LuaTypes.LUA_TUSERDATA)
                {
                    return this.extractValues[num];
                }
            }
            else if (paramType == typeof(LuaFunction))
            {
                if (types == LuaTypes.LUA_TFUNCTION)
                {
                    return this.extractValues[num];
                }
            }
            else
            {
                if (typeof(Delegate).IsAssignableFrom(paramType) && (types == LuaTypes.LUA_TFUNCTION))
                {
                    return new ExtractValue(new DelegateGenerator(this.translator, paramType).extractGenerated);
                }
                if (paramType.IsInterface && (types == LuaTypes.LUA_TTABLE))
                {
                    return new ExtractValue(new ClassGenerator(this.translator, paramType).extractGenerated);
                }
                if ((paramType.IsInterface || paramType.IsClass) && (types == LuaTypes.LUA_TNIL))
                {
                    return this.extractNetObject;
                }
                if (LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE)
                {
                    if (!LuaDLL.luaL_getmetafield(luaState, stackPos, "__index"))
                    {
                        return null;
                    }
                    object obj2 = this.translator.getNetObject(luaState, -1);
                    LuaDLL.lua_settop(luaState, -2);
                    if ((obj2 != null) && paramType.IsAssignableFrom(obj2.GetType()))
                    {
                        return this.extractNetObject;
                    }
                }
                else
                {
                    object obj3 = this.translator.getNetObject(luaState, stackPos);
                    if ((obj3 != null) && paramType.IsAssignableFrom(obj3.GetType()))
                    {
                        return this.extractNetObject;
                    }
                }
            }
            return null;
        }

        private object getAsBoolean(IntPtr luaState, int stackPos)
        {
            return LuaDLL.lua_toboolean(luaState, stackPos);
        }

        private object getAsByte(IntPtr luaState, int stackPos)
        {
            byte num = (byte) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsChar(IntPtr luaState, int stackPos)
        {
            char ch = (char) ((ushort) LuaDLL.lua_tonumber(luaState, stackPos));
            if ((ch == '\0') && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return ch;
        }

        private object getAsDecimal(IntPtr luaState, int stackPos)
        {
            decimal num = (decimal) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0M) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsDouble(IntPtr luaState, int stackPos)
        {
            double num = LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0.0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsFloat(IntPtr luaState, int stackPos)
        {
            float num = (float) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0f) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsFunction(IntPtr luaState, int stackPos)
        {
            return this.translator.getFunction(luaState, stackPos);
        }

        private object getAsInt(IntPtr luaState, int stackPos)
        {
            int num = (int) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsLong(IntPtr luaState, int stackPos)
        {
            long num = (long) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0L) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        public object getAsNetObject(IntPtr luaState, int stackPos)
        {
            object obj2 = this.translator.getNetObject(luaState, stackPos);
            if (((obj2 == null) && (LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE)) && LuaDLL.luaL_getmetafield(luaState, stackPos, "__index"))
            {
                if (LuaDLL.luaL_checkmetatable(luaState, -1))
                {
                    LuaDLL.lua_insert(luaState, stackPos);
                    LuaDLL.lua_remove(luaState, stackPos + 1);
                    return this.translator.getNetObject(luaState, stackPos);
                }
                LuaDLL.lua_settop(luaState, -2);
            }
            return obj2;
        }

        public object getAsObject(IntPtr luaState, int stackPos)
        {
            if ((LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE) && LuaDLL.luaL_getmetafield(luaState, stackPos, "__index"))
            {
                if (LuaDLL.luaL_checkmetatable(luaState, -1))
                {
                    LuaDLL.lua_insert(luaState, stackPos);
                    LuaDLL.lua_remove(luaState, stackPos + 1);
                }
                else
                {
                    LuaDLL.lua_settop(luaState, -2);
                }
            }
            return this.translator.getObject(luaState, stackPos);
        }

        private object getAsSbyte(IntPtr luaState, int stackPos)
        {
            sbyte num = (sbyte) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsShort(IntPtr luaState, int stackPos)
        {
            short num = (short) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsString(IntPtr luaState, int stackPos)
        {
            string str = LuaDLL.lua_tostring(luaState, stackPos);
            if ((str == "") && !LuaDLL.lua_isstring(luaState, stackPos))
            {
                return null;
            }
            return str;
        }

        private object getAsTable(IntPtr luaState, int stackPos)
        {
            return this.translator.getTable(luaState, stackPos);
        }

        private object getAsUint(IntPtr luaState, int stackPos)
        {
            uint num = (uint) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsUlong(IntPtr luaState, int stackPos)
        {
            ulong num = (ulong) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0L) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        private object getAsUserdata(IntPtr luaState, int stackPos)
        {
            return this.translator.getUserData(luaState, stackPos);
        }

        private object getAsUshort(IntPtr luaState, int stackPos)
        {
            ushort num = (ushort) LuaDLL.lua_tonumber(luaState, stackPos);
            if ((num == 0) && !LuaDLL.lua_isnumber(luaState, stackPos))
            {
                return null;
            }
            return num;
        }

        internal ExtractValue getExtractor(IReflect paramType)
        {
            return this.getExtractor(paramType.UnderlyingSystemType);
        }

        internal ExtractValue getExtractor(Type paramType)
        {
            if (paramType.IsByRef)
            {
                paramType = paramType.GetElementType();
            }
            long key = paramType.TypeHandle.Value.ToInt64();
            if (this.extractValues.ContainsKey(key))
            {
                return this.extractValues[key];
            }
            return this.extractNetObject;
        }
    }
}

