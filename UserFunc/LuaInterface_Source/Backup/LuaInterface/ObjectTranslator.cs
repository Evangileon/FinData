namespace LuaInterface
{
    using Lua511;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public class ObjectTranslator
    {
        private List<Assembly> assemblies;
        private LuaCSFunction getConstructorSigFunction;
        private LuaCSFunction getMethodSigFunction;
        private LuaCSFunction importTypeFunction;
        internal Lua interpreter;
        private LuaCSFunction loadAssemblyFunction;
        private MetaFunctions metaFunctions;
        private int nextObj;
        public readonly Dictionary<int, object> objects = new Dictionary<int, object>();
        public readonly Dictionary<object, int> objectsBackMap = new Dictionary<object, int>();
        internal EventHandlerContainer pendingEvents = new EventHandlerContainer();
        private LuaCSFunction registerTableFunction;
        internal CheckType typeChecker;
        private LuaCSFunction unregisterTableFunction;

        public ObjectTranslator(Lua interpreter, IntPtr luaState)
        {
            this.interpreter = interpreter;
            this.typeChecker = new CheckType(this);
            this.metaFunctions = new MetaFunctions(this);
            this.assemblies = new List<Assembly>();
            this.importTypeFunction = new LuaCSFunction(this.importType);
            this.loadAssemblyFunction = new LuaCSFunction(this.loadAssembly);
            this.registerTableFunction = new LuaCSFunction(this.registerTable);
            this.unregisterTableFunction = new LuaCSFunction(this.unregisterTable);
            this.getMethodSigFunction = new LuaCSFunction(this.getMethodSignature);
            this.getConstructorSigFunction = new LuaCSFunction(this.getConstructorSignature);
            this.createLuaObjectList(luaState);
            this.createIndexingMetaFunction(luaState);
            this.createBaseClassMetatable(luaState);
            this.createClassMetatable(luaState);
            this.createFunctionMetatable(luaState);
            this.setGlobalFunctions(luaState);
        }

        private int addObject(object obj)
        {
            int num = this.nextObj++;
            this.objects[num] = obj;
            this.objectsBackMap[obj] = num;
            return num;
        }

        internal void collectObject(int udata)
        {
            object obj2;
            if (this.objects.TryGetValue(udata, out obj2))
            {
                this.objects.Remove(udata);
                this.objectsBackMap.Remove(obj2);
            }
        }

        private void collectObject(object o, int udata)
        {
            this.objects.Remove(udata);
            this.objectsBackMap.Remove(o);
        }

        private void createBaseClassMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_searchbase");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__tostring");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__index");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.baseIndexFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__newindex");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createClassMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_class");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__tostring");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__index");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classIndexFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__newindex");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classNewindexFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__call");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.callConstructorFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createFunctionMetatable(IntPtr luaState)
        {
            LuaDLL.luaL_newmetatable(luaState, "luaNet_function");
            LuaDLL.lua_pushstring(luaState, "__gc");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_pushstring(luaState, "__call");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.execDelegateFunction);
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_settop(luaState, -2);
        }

        private void createIndexingMetaFunction(IntPtr luaState)
        {
            LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
            LuaDLL.luaL_dostring(luaState, MetaFunctions.luaIndexFunction);
            LuaDLL.lua_rawset(luaState, -10000);
        }

        private void createLuaObjectList(IntPtr luaState)
        {
            LuaDLL.lua_pushstring(luaState, "luaNet_objects");
            LuaDLL.lua_newtable(luaState);
            LuaDLL.lua_newtable(luaState);
            LuaDLL.lua_pushstring(luaState, "__mode");
            LuaDLL.lua_pushstring(luaState, "v");
            LuaDLL.lua_settable(luaState, -3);
            LuaDLL.lua_setmetatable(luaState, -2);
            LuaDLL.lua_settable(luaState, -10000);
        }

        internal Type FindType(string className)
        {
            foreach (Assembly assembly in this.assemblies)
            {
                Type type = assembly.GetType(className);
                if (type != null)
                {
                    return type;
                }
            }
            return null;
        }

        internal object getAsType(IntPtr luaState, int stackPos, Type paramType)
        {
            ExtractValue value2 = this.typeChecker.checkType(luaState, stackPos, paramType);
            if (value2 != null)
            {
                return value2(luaState, stackPos);
            }
            return null;
        }

        private int getConstructorSignature(IntPtr luaState)
        {
            IReflect targetType = null;
            int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num != -1)
            {
                targetType = (IReflect) this.objects[num];
            }
            if (targetType == null)
            {
                this.throwError(luaState, "get_constructor_bysig: first arg is invalid type reference");
            }
            Type[] types = new Type[LuaDLL.lua_gettop(luaState) - 1];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = this.FindType(LuaDLL.lua_tostring(luaState, i + 2));
            }
            try
            {
                ConstructorInfo constructor = targetType.UnderlyingSystemType.GetConstructor(types);
                this.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(this, null, targetType, constructor).call));
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception);
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        internal LuaFunction getFunction(IntPtr luaState, int index)
        {
            LuaDLL.lua_pushvalue(luaState, index);
            return new LuaFunction(LuaDLL.lua_ref(luaState, 1), this.interpreter);
        }

        private int getMethodSignature(IntPtr luaState)
        {
            IReflect type;
            object obj2;
            int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num != -1)
            {
                type = (IReflect) this.objects[num];
                obj2 = null;
            }
            else
            {
                obj2 = this.getRawNetObject(luaState, 1);
                if (obj2 == null)
                {
                    this.throwError(luaState, "get_method_bysig: first arg is not type or object reference");
                    LuaDLL.lua_pushnil(luaState);
                    return 1;
                }
                type = obj2.GetType();
            }
            string name = LuaDLL.lua_tostring(luaState, 2);
            Type[] types = new Type[LuaDLL.lua_gettop(luaState) - 2];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = this.FindType(LuaDLL.lua_tostring(luaState, i + 3));
            }
            try
            {
                MethodInfo method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase, null, types, null);
                this.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(this, obj2, type, method).call));
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception);
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        internal object getNetObject(IntPtr luaState, int index)
        {
            int num = LuaDLL.luanet_tonetobject(luaState, index);
            if (num != -1)
            {
                return this.objects[num];
            }
            return null;
        }

        internal object getObject(IntPtr luaState, int index)
        {
            switch (LuaDLL.lua_type(luaState, index))
            {
                case LuaTypes.LUA_TBOOLEAN:
                    return LuaDLL.lua_toboolean(luaState, index);

                case LuaTypes.LUA_TNUMBER:
                    return LuaDLL.lua_tonumber(luaState, index);

                case LuaTypes.LUA_TSTRING:
                    return LuaDLL.lua_tostring(luaState, index);

                case LuaTypes.LUA_TTABLE:
                    return this.getTable(luaState, index);

                case LuaTypes.LUA_TFUNCTION:
                    return this.getFunction(luaState, index);

                case LuaTypes.LUA_TUSERDATA:
                {
                    int num = LuaDLL.luanet_tonetobject(luaState, index);
                    if (num == -1)
                    {
                        return this.getUserData(luaState, index);
                    }
                    return this.objects[num];
                }
            }
            return null;
        }

        internal object getRawNetObject(IntPtr luaState, int index)
        {
            int num = LuaDLL.luanet_rawnetobj(luaState, index);
            if (num != -1)
            {
                return this.objects[num];
            }
            return null;
        }

        internal LuaTable getTable(IntPtr luaState, int index)
        {
            LuaDLL.lua_pushvalue(luaState, index);
            return new LuaTable(LuaDLL.lua_ref(luaState, 1), this.interpreter);
        }

        internal LuaUserData getUserData(IntPtr luaState, int index)
        {
            LuaDLL.lua_pushvalue(luaState, index);
            return new LuaUserData(LuaDLL.lua_ref(luaState, 1), this.interpreter);
        }

        private int importType(IntPtr luaState)
        {
            string className = LuaDLL.lua_tostring(luaState, 1);
            Type t = this.FindType(className);
            if (t != null)
            {
                this.pushType(luaState, t);
            }
            else
            {
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        private static bool IsILua(object o)
        {
            return ((o is ILuaGeneratedType) && (o.GetType().GetInterface("ILuaGeneratedType") != null));
        }

        private int loadAssembly(IntPtr luaState)
        {
            string partialName = LuaDLL.lua_tostring(luaState, 1);
            try
            {
                Assembly item = Assembly.LoadWithPartialName(partialName);
                try
                {
                    if (item == null)
                    {
                        item = Assembly.Load(AssemblyName.GetAssemblyName(partialName));
                    }
                }
                catch (Exception)
                {
                }
                if ((item != null) && !this.assemblies.Contains(item))
                {
                    this.assemblies.Add(item);
                }
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception);
            }
            return 0;
        }

        internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
        {
            return this.metaFunctions.matchParameters(luaState, method, ref methodCache);
        }

        internal object[] popValues(IntPtr luaState, int oldTop)
        {
            int num = LuaDLL.lua_gettop(luaState);
            if (oldTop == num)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            for (int i = oldTop + 1; i <= num; i++)
            {
                list.Add(this.getObject(luaState, i));
            }
            LuaDLL.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal object[] popValues(IntPtr luaState, int oldTop, Type[] popTypes)
        {
            int num2;
            int num = LuaDLL.lua_gettop(luaState);
            if (oldTop == num)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            if (popTypes[0] == typeof(void))
            {
                num2 = 1;
            }
            else
            {
                num2 = 0;
            }
            for (int i = oldTop + 1; i <= num; i++)
            {
                list.Add(this.getAsType(luaState, i, popTypes[num2]));
                num2++;
            }
            LuaDLL.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal void push(IntPtr luaState, object o)
        {
            if (o == null)
            {
                LuaDLL.lua_pushnil(luaState);
            }
            else if ((((o is sbyte) || (o is byte)) || ((o is short) || (o is ushort))) || ((((o is int) || (o is uint)) || ((o is long) || (o is float))) || (((o is ulong) || (o is decimal)) || (o is double))))
            {
                double number = Convert.ToDouble(o);
                LuaDLL.lua_pushnumber(luaState, number);
            }
            else if (o is char)
            {
                double num2 = (double) ((char) o);
                LuaDLL.lua_pushnumber(luaState, num2);
            }
            else if (o is string)
            {
                string str = (string) o;
                LuaDLL.lua_pushstring(luaState, str);
            }
            else if (o is bool)
            {
                bool flag = (bool) o;
                LuaDLL.lua_pushboolean(luaState, flag);
            }
            else if (IsILua(o))
            {
                ((ILuaGeneratedType) o).__luaInterface_getLuaTable().push(luaState);
            }
            else if (o is LuaTable)
            {
                ((LuaTable) o).push(luaState);
            }
            else if (o is LuaCSFunction)
            {
                this.pushFunction(luaState, (LuaCSFunction) o);
            }
            else if (o is LuaFunction)
            {
                ((LuaFunction) o).push(luaState);
            }
            else
            {
                this.pushObject(luaState, o, "luaNet_metatable");
            }
        }

        internal void pushFunction(IntPtr luaState, LuaCSFunction func)
        {
            this.pushObject(luaState, func, "luaNet_function");
        }

        private void pushNewObject(IntPtr luaState, object o, int index, string metatable)
        {
            if (metatable == "luaNet_metatable")
            {
                LuaDLL.luaL_getmetatable(luaState, o.GetType().AssemblyQualifiedName);
                if (LuaDLL.lua_isnil(luaState, -1))
                {
                    LuaDLL.lua_settop(luaState, -2);
                    LuaDLL.luaL_newmetatable(luaState, o.GetType().AssemblyQualifiedName);
                    LuaDLL.lua_pushstring(luaState, "cache");
                    LuaDLL.lua_newtable(luaState);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
                    LuaDLL.lua_pushnumber(luaState, 1.0);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__index");
                    LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
                    LuaDLL.lua_rawget(luaState, -10000);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__gc");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__tostring");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
                    LuaDLL.lua_rawset(luaState, -3);
                    LuaDLL.lua_pushstring(luaState, "__newindex");
                    LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction);
                    LuaDLL.lua_rawset(luaState, -3);
                }
            }
            else
            {
                LuaDLL.luaL_getmetatable(luaState, metatable);
            }
            LuaDLL.luaL_getmetatable(luaState, "luaNet_objects");
            LuaDLL.luanet_newudata(luaState, index);
            LuaDLL.lua_pushvalue(luaState, -3);
            LuaDLL.lua_remove(luaState, -4);
            LuaDLL.lua_setmetatable(luaState, -2);
            LuaDLL.lua_pushvalue(luaState, -1);
            LuaDLL.lua_rawseti(luaState, -3, index);
            LuaDLL.lua_remove(luaState, -2);
        }

        internal void pushObject(IntPtr luaState, object o, string metatable)
        {
            int num = -1;
            if (o == null)
            {
                LuaDLL.lua_pushnil(luaState);
            }
            else
            {
                if (this.objectsBackMap.TryGetValue(o, out num))
                {
                    LuaDLL.luaL_getmetatable(luaState, "luaNet_objects");
                    LuaDLL.lua_rawgeti(luaState, -1, num);
                    if (LuaDLL.lua_type(luaState, -1) != LuaTypes.LUA_TNIL)
                    {
                        LuaDLL.lua_remove(luaState, -2);
                        return;
                    }
                    LuaDLL.lua_remove(luaState, -1);
                    LuaDLL.lua_remove(luaState, -1);
                    this.collectObject(o, num);
                }
                num = this.addObject(o);
                this.pushNewObject(luaState, o, num, metatable);
            }
        }

        internal void pushType(IntPtr luaState, Type t)
        {
            this.pushObject(luaState, new ProxyType(t), "luaNet_class");
        }

        private int registerTable(IntPtr luaState)
        {
            if (LuaDLL.lua_type(luaState, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaTable luaTable = this.getTable(luaState, 1);
                string className = LuaDLL.lua_tostring(luaState, 2);
                if (className != null)
                {
                    Type klass = this.FindType(className);
                    if (klass != null)
                    {
                        object classInstance = CodeGeneration.Instance.GetClassInstance(klass, luaTable);
                        this.pushObject(luaState, classInstance, "luaNet_metatable");
                        LuaDLL.lua_newtable(luaState);
                        LuaDLL.lua_pushstring(luaState, "__index");
                        LuaDLL.lua_pushvalue(luaState, -3);
                        LuaDLL.lua_settable(luaState, -3);
                        LuaDLL.lua_pushstring(luaState, "__newindex");
                        LuaDLL.lua_pushvalue(luaState, -3);
                        LuaDLL.lua_settable(luaState, -3);
                        LuaDLL.lua_setmetatable(luaState, 1);
                        LuaDLL.lua_pushstring(luaState, "base");
                        int index = this.addObject(classInstance);
                        this.pushNewObject(luaState, classInstance, index, "luaNet_searchbase");
                        LuaDLL.lua_rawset(luaState, 1);
                    }
                    else
                    {
                        this.throwError(luaState, "register_table: can not find superclass '" + className + "'");
                    }
                }
                else
                {
                    this.throwError(luaState, "register_table: superclass name can not be null");
                }
            }
            else
            {
                this.throwError(luaState, "register_table: first arg is not a table");
            }
            return 0;
        }

        internal int returnValues(IntPtr luaState, object[] returnValues)
        {
            if (!LuaDLL.lua_checkstack(luaState, returnValues.Length + 5))
            {
                return 0;
            }
            for (int i = 0; i < returnValues.Length; i++)
            {
                this.push(luaState, returnValues[i]);
            }
            return returnValues.Length;
        }

        private void setGlobalFunctions(IntPtr luaState)
        {
            LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.indexFunction);
            LuaDLL.lua_setglobal(luaState, "get_object_member");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.importTypeFunction);
            LuaDLL.lua_setglobal(luaState, "import_type");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.loadAssemblyFunction);
            LuaDLL.lua_setglobal(luaState, "load_assembly");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.registerTableFunction);
            LuaDLL.lua_setglobal(luaState, "make_object");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.unregisterTableFunction);
            LuaDLL.lua_setglobal(luaState, "free_object");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.getMethodSigFunction);
            LuaDLL.lua_setglobal(luaState, "get_method_bysig");
            LuaDLL.lua_pushstdcallcfunction(luaState, this.getConstructorSigFunction);
            LuaDLL.lua_setglobal(luaState, "get_constructor_bysig");
        }

        internal void throwError(IntPtr luaState, object e)
        {
            if (e is string)
            {
                int oldTop = LuaDLL.lua_gettop(luaState);
                LuaDLL.luaL_where(luaState, 2);
                object[] objArray = this.popValues(luaState, oldTop);
                if (objArray.Length > 0)
                {
                    e = objArray[0].ToString() + e;
                }
            }
            this.push(luaState, e);
            LuaDLL.lua_error(luaState);
        }

        private int unregisterTable(IntPtr luaState)
        {
            try
            {
                if (LuaDLL.lua_getmetatable(luaState, 1) != 0)
                {
                    LuaDLL.lua_pushstring(luaState, "__index");
                    LuaDLL.lua_gettable(luaState, -2);
                    object obj2 = this.getRawNetObject(luaState, -1);
                    if (obj2 == null)
                    {
                        this.throwError(luaState, "unregister_table: arg is not valid table");
                    }
                    FieldInfo field = obj2.GetType().GetField("__luaInterface_luaTable");
                    if (field == null)
                    {
                        this.throwError(luaState, "unregister_table: arg is not valid table");
                    }
                    field.SetValue(obj2, null);
                    LuaDLL.lua_pushnil(luaState);
                    LuaDLL.lua_setmetatable(luaState, 1);
                    LuaDLL.lua_pushstring(luaState, "base");
                    LuaDLL.lua_pushnil(luaState);
                    LuaDLL.lua_settable(luaState, 1);
                }
                else
                {
                    this.throwError(luaState, "unregister_table: arg is not valid table");
                }
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception.Message);
            }
            return 0;
        }
    }
}

