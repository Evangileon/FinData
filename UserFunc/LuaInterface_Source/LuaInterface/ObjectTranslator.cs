namespace LuaInterface
{
    using LuaInterface;
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
            LuaJIT.luaL_newmetatable(luaState, "luaNet_searchbase");
            LuaJIT.lua_pushstring(luaState, "__gc");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__tostring");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__index");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.baseIndexFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__newindex");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_settop(luaState, -2);
        }

        private void createClassMetatable(IntPtr luaState)
        {
            LuaJIT.luaL_newmetatable(luaState, "luaNet_class");
            LuaJIT.lua_pushstring(luaState, "__gc");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__tostring");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__index");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.classIndexFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__newindex");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.classNewindexFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__call");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.callConstructorFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_settop(luaState, -2);
        }

        private void createFunctionMetatable(IntPtr luaState)
        {
            LuaJIT.luaL_newmetatable(luaState, "luaNet_function");
            LuaJIT.lua_pushstring(luaState, "__gc");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_pushstring(luaState, "__call");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.execDelegateFunction);
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_settop(luaState, -2);
        }

        private void createIndexingMetaFunction(IntPtr luaState)
        {
            LuaJIT.lua_pushstring(luaState, "luaNet_indexfunction");
            LuaJIT.luaL_dostring(luaState, MetaFunctions.luaIndexFunction);
            LuaJIT.lua_rawset(luaState, -10000);
        }

        private void createLuaObjectList(IntPtr luaState)
        {
            LuaJIT.lua_pushstring(luaState, "luaNet_objects");
            LuaJIT.lua_newtable(luaState);
            LuaJIT.lua_newtable(luaState);
            LuaJIT.lua_pushstring(luaState, "__mode");
            LuaJIT.lua_pushstring(luaState, "v");
            LuaJIT.lua_settable(luaState, -3);
            LuaJIT.lua_setmetatable(luaState, -2);
            LuaJIT.lua_settable(luaState, -10000);
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
            int num = LuaJIT.luanet_checkudata(luaState, 1, "luaNet_class");
            if (num != -1)
            {
                targetType = (IReflect) this.objects[num];
            }
            if (targetType == null)
            {
                this.throwError(luaState, "get_constructor_bysig: first arg is invalid type reference");
            }
            Type[] types = new Type[LuaJIT.lua_gettop(luaState) - 1];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = this.FindType(LuaJIT.lua_tostring(luaState, i + 2));
            }
            try
            {
                ConstructorInfo constructor = targetType.UnderlyingSystemType.GetConstructor(types);
                this.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(this, null, targetType, constructor).call));
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception);
                LuaJIT.lua_pushnil(luaState);
            }
            return 1;
        }

        internal LuaFunction getFunction(IntPtr luaState, int index)
        {
            LuaJIT.lua_pushvalue(luaState, index);
            return new LuaFunction(LuaJIT.lua_ref(luaState, 1), this.interpreter);
        }

        private int getMethodSignature(IntPtr luaState)
        {
            IReflect type;
            object obj2;
            int num = LuaJIT.luanet_checkudata(luaState, 1, "luaNet_class");
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
                    LuaJIT.lua_pushnil(luaState);
                    return 1;
                }
                type = obj2.GetType();
            }
            string name = LuaJIT.lua_tostring(luaState, 2);
            Type[] types = new Type[LuaJIT.lua_gettop(luaState) - 2];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = this.FindType(LuaJIT.lua_tostring(luaState, i + 3));
            }
            try
            {
                MethodInfo method = type.GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase, null, types, null);
                this.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(this, obj2, type, method).call));
            }
            catch (Exception exception)
            {
                this.throwError(luaState, exception);
                LuaJIT.lua_pushnil(luaState);
            }
            return 1;
        }

        internal object getNetObject(IntPtr luaState, int index)
        {
            int num = LuaJIT.luanet_tonetobject(luaState, index);
            if (num != -1)
            {
                return this.objects[num];
            }
            return null;
        }

        internal object getObject(IntPtr luaState, int index)
        {
            switch (LuaJIT.lua_type(luaState, index))
            {
                case LuaTypes.LUA_TBOOLEAN:
                    return LuaJIT.lua_toboolean(luaState, index);

                case LuaTypes.LUA_TNUMBER:
                    return LuaJIT.lua_tonumber(luaState, index);

                case LuaTypes.LUA_TSTRING:
                    return LuaJIT.lua_tostring(luaState, index);

                case LuaTypes.LUA_TTABLE:
                    return this.getTable(luaState, index);

                case LuaTypes.LUA_TFUNCTION:
                    return this.getFunction(luaState, index);

                case LuaTypes.LUA_TUSERDATA:
                {
                    int num = LuaJIT.luanet_tonetobject(luaState, index);
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
            int num = LuaJIT.luanet_rawnetobj(luaState, index);
            if (num != -1)
            {
                return this.objects[num];
            }
            return null;
        }

        internal LuaTable getTable(IntPtr luaState, int index)
        {
            LuaJIT.lua_pushvalue(luaState, index);
            return new LuaTable(LuaJIT.lua_ref(luaState, 1), this.interpreter);
        }

        internal LuaUserData getUserData(IntPtr luaState, int index)
        {
            LuaJIT.lua_pushvalue(luaState, index);
            return new LuaUserData(LuaJIT.lua_ref(luaState, 1), this.interpreter);
        }

        private int importType(IntPtr luaState)
        {
            string className = LuaJIT.lua_tostring(luaState, 1);
            Type t = this.FindType(className);
            if (t != null)
            {
                this.pushType(luaState, t);
            }
            else
            {
                LuaJIT.lua_pushnil(luaState);
            }
            return 1;
        }

        private static bool IsILua(object o)
        {
            return ((o is ILuaGeneratedType) && (o.GetType().GetInterface("ILuaGeneratedType") != null));
        }

        private int loadAssembly(IntPtr luaState)
        {
            string partialName = LuaJIT.lua_tostring(luaState, 1);
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
            int num = LuaJIT.lua_gettop(luaState);
            if (oldTop == num)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            for (int i = oldTop + 1; i <= num; i++)
            {
                list.Add(this.getObject(luaState, i));
            }
            LuaJIT.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal object[] popValues(IntPtr luaState, int oldTop, Type[] popTypes)
        {
            int num2;
            int num = LuaJIT.lua_gettop(luaState);
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
            LuaJIT.lua_settop(luaState, oldTop);
            return list.ToArray();
        }

        internal void push(IntPtr luaState, object o)
        {
            if (o == null)
            {
                LuaJIT.lua_pushnil(luaState);
            }
            else if ((((o is sbyte) || (o is byte)) || ((o is short) || (o is ushort))) || ((((o is int) || (o is uint)) || ((o is long) || (o is float))) || (((o is ulong) || (o is decimal)) || (o is double))))
            {
                double number = Convert.ToDouble(o);
                LuaJIT.lua_pushnumber(luaState, number);
            }
            else if (o is char)
            {
                double num2 = (double) ((char) o);
                LuaJIT.lua_pushnumber(luaState, num2);
            }
            else if (o is string)
            {
                string str = (string) o;
                LuaJIT.lua_pushstring(luaState, str);
            }
            else if (o is bool)
            {
                bool flag = (bool) o;
                LuaJIT.lua_pushboolean(luaState, flag);
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
                LuaJIT.luaL_getmetatable(luaState, o.GetType().AssemblyQualifiedName);
                if (LuaJIT.lua_isnil(luaState, -1))
                {
                    LuaJIT.lua_settop(luaState, -2);
                    LuaJIT.luaL_newmetatable(luaState, o.GetType().AssemblyQualifiedName);
                    LuaJIT.lua_pushstring(luaState, "cache");
                    LuaJIT.lua_newtable(luaState);
                    LuaJIT.lua_rawset(luaState, -3);
                    LuaJIT.lua_pushlightuserdata(luaState, LuaJIT.luanet_gettag());
                    LuaJIT.lua_pushnumber(luaState, 1.0);
                    LuaJIT.lua_rawset(luaState, -3);
                    LuaJIT.lua_pushstring(luaState, "__index");
                    LuaJIT.lua_pushstring(luaState, "luaNet_indexfunction");
                    LuaJIT.lua_rawget(luaState, -10000);
                    LuaJIT.lua_rawset(luaState, -3);
                    LuaJIT.lua_pushstring(luaState, "__gc");
                    LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction);
                    LuaJIT.lua_rawset(luaState, -3);
                    LuaJIT.lua_pushstring(luaState, "__tostring");
                    LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction);
                    LuaJIT.lua_rawset(luaState, -3);
                    LuaJIT.lua_pushstring(luaState, "__newindex");
                    LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction);
                    LuaJIT.lua_rawset(luaState, -3);
                }
            }
            else
            {
                LuaJIT.luaL_getmetatable(luaState, metatable);
            }
            LuaJIT.luaL_getmetatable(luaState, "luaNet_objects");
            LuaJIT.luanet_newudata(luaState, index);
            LuaJIT.lua_pushvalue(luaState, -3);
            LuaJIT.lua_remove(luaState, -4);
            LuaJIT.lua_setmetatable(luaState, -2);
            LuaJIT.lua_pushvalue(luaState, -1);
            LuaJIT.lua_rawseti(luaState, -3, index);
            LuaJIT.lua_remove(luaState, -2);
        }

        internal void pushObject(IntPtr luaState, object o, string metatable)
        {
            int num = -1;
            if (o == null)
            {
                LuaJIT.lua_pushnil(luaState);
            }
            else
            {
                if (this.objectsBackMap.TryGetValue(o, out num))
                {
                    LuaJIT.luaL_getmetatable(luaState, "luaNet_objects");
                    LuaJIT.lua_rawgeti(luaState, -1, num);
                    if (LuaJIT.lua_type(luaState, -1) != LuaTypes.LUA_TNIL)
                    {
                        LuaJIT.lua_remove(luaState, -2);
                        return;
                    }
                    LuaJIT.lua_remove(luaState, -1);
                    LuaJIT.lua_remove(luaState, -1);
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
            if (LuaJIT.lua_type(luaState, 1) == LuaTypes.LUA_TTABLE)
            {
                LuaTable luaTable = this.getTable(luaState, 1);
                string className = LuaJIT.lua_tostring(luaState, 2);
                if (className != null)
                {
                    Type klass = this.FindType(className);
                    if (klass != null)
                    {
                        object classInstance = CodeGeneration.Instance.GetClassInstance(klass, luaTable);
                        this.pushObject(luaState, classInstance, "luaNet_metatable");
                        LuaJIT.lua_newtable(luaState);
                        LuaJIT.lua_pushstring(luaState, "__index");
                        LuaJIT.lua_pushvalue(luaState, -3);
                        LuaJIT.lua_settable(luaState, -3);
                        LuaJIT.lua_pushstring(luaState, "__newindex");
                        LuaJIT.lua_pushvalue(luaState, -3);
                        LuaJIT.lua_settable(luaState, -3);
                        LuaJIT.lua_setmetatable(luaState, 1);
                        LuaJIT.lua_pushstring(luaState, "base");
                        int index = this.addObject(classInstance);
                        this.pushNewObject(luaState, classInstance, index, "luaNet_searchbase");
                        LuaJIT.lua_rawset(luaState, 1);
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
            if (!LuaJIT.lua_checkstack(luaState, returnValues.Length + 5))
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
            LuaJIT.lua_pushstdcallcfunction(luaState, this.metaFunctions.indexFunction);
            LuaJIT.lua_setglobal(luaState, "get_object_member");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.importTypeFunction);
            LuaJIT.lua_setglobal(luaState, "import_type");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.loadAssemblyFunction);
            LuaJIT.lua_setglobal(luaState, "load_assembly");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.registerTableFunction);
            LuaJIT.lua_setglobal(luaState, "make_object");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.unregisterTableFunction);
            LuaJIT.lua_setglobal(luaState, "free_object");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.getMethodSigFunction);
            LuaJIT.lua_setglobal(luaState, "get_method_bysig");
            LuaJIT.lua_pushstdcallcfunction(luaState, this.getConstructorSigFunction);
            LuaJIT.lua_setglobal(luaState, "get_constructor_bysig");
        }

        internal void throwError(IntPtr luaState, object e)
        {
            if (e is string)
            {
                int oldTop = LuaJIT.lua_gettop(luaState);
                LuaJIT.luaL_where(luaState, 2);
                object[] objArray = this.popValues(luaState, oldTop);
                if (objArray.Length > 0)
                {
                    e = objArray[0].ToString() + e;
                }
            }
            this.push(luaState, e);
            LuaJIT.lua_error(luaState);
        }

        private int unregisterTable(IntPtr luaState)
        {
            try
            {
                if (LuaJIT.lua_getmetatable(luaState, 1) != 0)
                {
                    LuaJIT.lua_pushstring(luaState, "__index");
                    LuaJIT.lua_gettable(luaState, -2);
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
                    LuaJIT.lua_pushnil(luaState);
                    LuaJIT.lua_setmetatable(luaState, 1);
                    LuaJIT.lua_pushstring(luaState, "base");
                    LuaJIT.lua_pushnil(luaState);
                    LuaJIT.lua_settable(luaState, 1);
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

