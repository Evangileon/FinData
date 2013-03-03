namespace LuaInterface
{
    using Lua511;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class MetaFunctions
    {
        internal LuaCSFunction baseIndexFunction;
        internal LuaCSFunction callConstructorFunction;
        internal LuaCSFunction classIndexFunction;
        internal LuaCSFunction classNewindexFunction;
        internal LuaCSFunction execDelegateFunction;
        internal LuaCSFunction gcFunction;
        internal LuaCSFunction indexFunction;
        internal static string luaIndexFunction = "local function index(obj,name)\n  local meta=getmetatable(obj)\n  local cached=meta.cache[name]\n  if cached~=nil  then\n    return cached\n  else\n    local value,isFunc=get_object_member(obj,name)\n    if isFunc then\n      meta.cache[name]=value\n    end\n    return value\n  end\nend\nreturn index";
        private Hashtable memberCache = new Hashtable();
        internal LuaCSFunction newindexFunction;
        internal LuaCSFunction toStringFunction;
        private ObjectTranslator translator;

        public MetaFunctions(ObjectTranslator translator)
        {
            this.translator = translator;
            this.gcFunction = new LuaCSFunction(this.collectObject);
            this.toStringFunction = new LuaCSFunction(this.toString);
            this.indexFunction = new LuaCSFunction(this.getMethod);
            this.newindexFunction = new LuaCSFunction(this.setFieldOrProperty);
            this.baseIndexFunction = new LuaCSFunction(this.getBaseMethod);
            this.callConstructorFunction = new LuaCSFunction(this.callConstructor);
            this.classIndexFunction = new LuaCSFunction(this.getClassMethod);
            this.classNewindexFunction = new LuaCSFunction(this.setClassFieldOrProperty);
            this.execDelegateFunction = new LuaCSFunction(this.runFunctionDelegate);
        }

        private bool _IsTypeCorrect(IntPtr luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
        {
            try
            {
                ExtractValue value2;
                extractValue = value2 = this.translator.typeChecker.checkType(luaState, currentLuaParam, currentNetParam.ParameterType);
                return (value2 != null);
            }
            catch
            {
                extractValue = null;
                return false;
            }
        }

        private int callConstructor(IntPtr luaState)
        {
            MethodCache methodCache = new MethodCache();
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                this.translator.throwError(luaState, "trying to call constructor on an invalid type reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            IReflect reflect = (IReflect) obj2;
            LuaDLL.lua_remove(luaState, 1);
            ConstructorInfo[] constructors = reflect.UnderlyingSystemType.GetConstructors();
            foreach (ConstructorInfo info in constructors)
            {
                if (this.matchParameters(luaState, info, ref methodCache))
                {
                    try
                    {
                        this.translator.push(luaState, info.Invoke(methodCache.args));
                    }
                    catch (TargetInvocationException exception)
                    {
                        this.ThrowError(luaState, exception);
                        LuaDLL.lua_pushnil(luaState);
                    }
                    catch
                    {
                        LuaDLL.lua_pushnil(luaState);
                    }
                    return 1;
                }
            }
            string str = (constructors.Length == 0) ? "unknown" : constructors[0].Name;
            this.translator.throwError(luaState, string.Format("{0} does not contain constructor({1}) argument match", reflect.UnderlyingSystemType, str));
            LuaDLL.lua_pushnil(luaState);
            return 1;
        }

        private object checkMemberCache(Hashtable memberCache, IReflect objType, string memberName)
        {
            Hashtable hashtable = (Hashtable) memberCache[objType];
            if (hashtable != null)
            {
                return hashtable[memberName];
            }
            return null;
        }

        private int collectObject(IntPtr luaState)
        {
            int udata = LuaDLL.luanet_rawnetobj(luaState, 1);
            if (udata != -1)
            {
                this.translator.collectObject(udata);
            }
            return 0;
        }

        public static void dumpStack(ObjectTranslator translator, IntPtr luaState)
        {
            int num = LuaDLL.lua_gettop(luaState);
            for (int i = 1; i <= num; i++)
            {
                LuaTypes type = LuaDLL.lua_type(luaState, i);
                if (type != LuaTypes.LUA_TTABLE)
                {
                    LuaDLL.lua_typename(luaState, type);
                }
                LuaDLL.lua_tostring(luaState, i);
                if (type == LuaTypes.LUA_TUSERDATA)
                {
                    translator.getRawNetObject(luaState, i).ToString();
                }
            }
        }

        private int getBaseMethod(IntPtr luaState)
        {
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if (obj2 == null)
            {
                this.translator.throwError(luaState, "trying to index an invalid object reference");
                LuaDLL.lua_pushnil(luaState);
                LuaDLL.lua_pushboolean(luaState, false);
                return 2;
            }
            string methodName = LuaDLL.lua_tostring(luaState, 2);
            if (methodName == null)
            {
                LuaDLL.lua_pushnil(luaState);
                LuaDLL.lua_pushboolean(luaState, false);
                return 2;
            }
            this.getMember(luaState, obj2.GetType(), obj2, "__luaInterface_base_" + methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
            LuaDLL.lua_settop(luaState, -2);
            if (LuaDLL.lua_type(luaState, -1) == LuaTypes.LUA_TNIL)
            {
                LuaDLL.lua_settop(luaState, -2);
                return this.getMember(luaState, obj2.GetType(), obj2, methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
            }
            LuaDLL.lua_pushboolean(luaState, false);
            return 2;
        }

        private int getClassMethod(IntPtr luaState)
        {
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                this.translator.throwError(luaState, "trying to index an invalid type reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            IReflect objType = (IReflect) obj2;
            if (LuaDLL.lua_isnumber(luaState, 2))
            {
                int length = (int) LuaDLL.lua_tonumber(luaState, 2);
                this.translator.push(luaState, Array.CreateInstance(objType.UnderlyingSystemType, length));
                return 1;
            }
            string methodName = LuaDLL.lua_tostring(luaState, 2);
            if (methodName == null)
            {
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            return this.getMember(luaState, objType, null, methodName, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.IgnoreCase);
        }

        private int getMember(IntPtr luaState, IReflect objType, object obj, string methodName, BindingFlags bindingType)
        {
            bool flag = false;
            MemberInfo member = null;
            object obj2 = this.checkMemberCache(this.memberCache, objType, methodName);
            if (obj2 is LuaCSFunction)
            {
                this.translator.pushFunction(luaState, (LuaCSFunction) obj2);
                this.translator.push(luaState, true);
                return 2;
            }
            if (obj2 != null)
            {
                member = (MemberInfo) obj2;
            }
            else
            {
                MemberInfo[] infoArray = objType.GetMember(methodName, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
                if (infoArray.Length > 0)
                {
                    member = infoArray[0];
                }
                else
                {
                    infoArray = objType.GetMember(methodName, ((bindingType | BindingFlags.Static) | BindingFlags.Public) | BindingFlags.IgnoreCase);
                    if (infoArray.Length > 0)
                    {
                        member = infoArray[0];
                        flag = true;
                    }
                }
            }
            if (member != null)
            {
                if (member.MemberType == MemberTypes.Field)
                {
                    FieldInfo info2 = (FieldInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    try
                    {
                        this.translator.push(luaState, info2.GetValue(obj));
                    }
                    catch
                    {
                        LuaDLL.lua_pushnil(luaState);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo info3 = (PropertyInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    try
                    {
                        object o = info3.GetValue(obj, null);
                        this.translator.push(luaState, o);
                    }
                    catch (ArgumentException)
                    {
                        if ((objType is Type) && (((Type) objType) != typeof(object)))
                        {
                            return this.getMember(luaState, ((Type) objType).BaseType, obj, methodName, bindingType);
                        }
                        LuaDLL.lua_pushnil(luaState);
                    }
                    catch (TargetInvocationException exception)
                    {
                        this.ThrowError(luaState, exception);
                        LuaDLL.lua_pushnil(luaState);
                    }
                }
                else if (member.MemberType == MemberTypes.Event)
                {
                    EventInfo eventInfo = (EventInfo) member;
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    this.translator.push(luaState, new RegisterEventHandler(this.translator.pendingEvents, obj, eventInfo));
                }
                else if (!flag)
                {
                    if (member.MemberType != MemberTypes.NestedType)
                    {
                        LuaCSFunction function = new LuaCSFunction(new LuaMethodWrapper(this.translator, objType, methodName, bindingType).call);
                        if (obj2 == null)
                        {
                            this.setMemberCache(this.memberCache, objType, methodName, function);
                        }
                        this.translator.pushFunction(luaState, function);
                        this.translator.push(luaState, true);
                        return 2;
                    }
                    if (obj2 == null)
                    {
                        this.setMemberCache(this.memberCache, objType, methodName, member);
                    }
                    string name = member.Name;
                    string className = member.DeclaringType.FullName + "+" + name;
                    Type t = this.translator.FindType(className);
                    this.translator.pushType(luaState, t);
                }
                else
                {
                    this.translator.throwError(luaState, "can't pass instance to static method " + methodName);
                    LuaDLL.lua_pushnil(luaState);
                }
            }
            else
            {
                this.translator.throwError(luaState, "unknown member name " + methodName);
                LuaDLL.lua_pushnil(luaState);
            }
            this.translator.push(luaState, false);
            return 2;
        }

        private int getMethod(IntPtr luaState)
        {
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if (obj2 == null)
            {
                this.translator.throwError(luaState, "trying to index an invalid object reference");
                LuaDLL.lua_pushnil(luaState);
                return 1;
            }
            object obj3 = this.translator.getObject(luaState, 2);
            obj3.GetType();
            string methodName = obj3 as string;
            Type objType = obj2.GetType();
            try
            {
                if ((methodName != null) && this.isMemberPresent(objType, methodName))
                {
                    return this.getMember(luaState, objType, obj2, methodName, BindingFlags.Instance | BindingFlags.IgnoreCase);
                }
            }
            catch
            {
            }
            if (objType.IsArray && (obj3 is double))
            {
                object[] objArray = (object[]) obj2;
                this.translator.push(luaState, objArray[(int) ((double) obj3)]);
            }
            else
            {
                foreach (MethodInfo info in objType.GetMethods())
                {
                    if ((info.Name == "get_Item") && (info.GetParameters().Length == 1))
                    {
                        MethodInfo info2 = info;
                        ParameterInfo[] infoArray2 = (info2 != null) ? info2.GetParameters() : null;
                        if ((infoArray2 == null) || (infoArray2.Length != 1))
                        {
                            this.translator.throwError(luaState, "method not found (or no indexer): " + obj3);
                            LuaDLL.lua_pushnil(luaState);
                        }
                        else
                        {
                            obj3 = this.translator.getAsType(luaState, 2, infoArray2[0].ParameterType);
                            object[] parameters = new object[] { obj3 };
                            try
                            {
                                object o = info2.Invoke(obj2, parameters);
                                this.translator.push(luaState, o);
                            }
                            catch (TargetInvocationException exception)
                            {
                                if (exception.InnerException is KeyNotFoundException)
                                {
                                    this.translator.throwError(luaState, "key '" + obj3 + "' not found ");
                                }
                                else
                                {
                                    this.translator.throwError(luaState, string.Concat(new object[] { "exception indexing '", obj3, "' ", exception.Message }));
                                }
                                LuaDLL.lua_pushnil(luaState);
                            }
                        }
                    }
                }
            }
            LuaDLL.lua_pushboolean(luaState, false);
            return 2;
        }

        private bool isMemberPresent(IReflect objType, string methodName)
        {
            return ((this.checkMemberCache(this.memberCache, objType, methodName) != null) || (objType.GetMember(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.IgnoreCase).Length > 0));
        }

        internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
        {
            bool flag = true;
            ParameterInfo[] parameters = method.GetParameters();
            int currentLuaParam = 1;
            int num2 = LuaDLL.lua_gettop(luaState);
            ArrayList list = new ArrayList();
            List<int> list2 = new List<int>();
            List<MethodArgs> list3 = new List<MethodArgs>();
            foreach (ParameterInfo info in parameters)
            {
                if (!info.IsIn && info.IsOut)
                {
                    list2.Add(list.Add(null));
                }
                else
                {
                    ExtractValue value2;
                    if (currentLuaParam > num2)
                    {
                        if (info.IsOptional)
                        {
                            list.Add(info.DefaultValue);
                            goto Label_0105;
                        }
                        flag = false;
                        break;
                    }
                    if (this._IsTypeCorrect(luaState, currentLuaParam, info, out value2))
                    {
                        int item = list.Add(value2(luaState, currentLuaParam));
                        MethodArgs args = new MethodArgs();
                        args.index = item;
                        args.extractValue = value2;
                        list3.Add(args);
                        if (info.ParameterType.IsByRef)
                        {
                            list2.Add(item);
                        }
                        currentLuaParam++;
                    }
                    else if (info.IsOptional)
                    {
                        list.Add(info.DefaultValue);
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                Label_0105:;
                }
            }
            if (currentLuaParam != (num2 + 1))
            {
                flag = false;
            }
            if (flag)
            {
                methodCache.args = list.ToArray();
                methodCache.cachedMethod = method;
                methodCache.outList = list2.ToArray();
                methodCache.argTypes = list3.ToArray();
            }
            return flag;
        }

        private int runFunctionDelegate(IntPtr luaState)
        {
            LuaCSFunction function = (LuaCSFunction) this.translator.getRawNetObject(luaState, 1);
            LuaDLL.lua_remove(luaState, 1);
            return function(luaState);
        }

        private int setClassFieldOrProperty(IntPtr luaState)
        {
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if ((obj2 == null) || !(obj2 is IReflect))
            {
                this.translator.throwError(luaState, "trying to index an invalid type reference");
                return 0;
            }
            IReflect targetType = (IReflect) obj2;
            return this.setMember(luaState, targetType, null, BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.IgnoreCase);
        }

        private int setFieldOrProperty(IntPtr luaState)
        {
            string str;
            object target = this.translator.getRawNetObject(luaState, 1);
            if (target == null)
            {
                this.translator.throwError(luaState, "trying to index and invalid object reference");
                return 0;
            }
            Type targetType = target.GetType();
            if (!this.trySetMember(luaState, targetType, target, BindingFlags.Instance | BindingFlags.IgnoreCase, out str))
            {
                try
                {
                    if (targetType.IsArray && LuaDLL.lua_isnumber(luaState, 2))
                    {
                        int index = (int) LuaDLL.lua_tonumber(luaState, 2);
                        Array array = (Array) target;
                        object obj3 = this.translator.getAsType(luaState, 3, array.GetType().GetElementType());
                        array.SetValue(obj3, index);
                    }
                    else
                    {
                        MethodInfo method = targetType.GetMethod("set_Item");
                        if (method != null)
                        {
                            ParameterInfo[] parameters = method.GetParameters();
                            Type parameterType = parameters[1].ParameterType;
                            object obj4 = this.translator.getAsType(luaState, 3, parameterType);
                            Type paramType = parameters[0].ParameterType;
                            object obj5 = this.translator.getAsType(luaState, 2, paramType);
                            object[] objArray = new object[] { obj5, obj4 };
                            method.Invoke(target, objArray);
                        }
                        else
                        {
                            this.translator.throwError(luaState, str);
                        }
                    }
                }
                catch (SEHException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    this.ThrowError(luaState, exception);
                }
            }
            return 0;
        }

        private int setMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType)
        {
            string str;
            if (!this.trySetMember(luaState, targetType, target, bindingType, out str))
            {
                this.translator.throwError(luaState, str);
            }
            return 0;
        }

        private void setMemberCache(Hashtable memberCache, IReflect objType, string memberName, object member)
        {
            Hashtable hashtable = (Hashtable) memberCache[objType];
            if (hashtable == null)
            {
                hashtable = new Hashtable();
                memberCache[objType] = hashtable;
            }
            hashtable[memberName] = member;
        }

        private void ThrowError(IntPtr luaState, Exception e)
        {
            TargetInvocationException exception = e as TargetInvocationException;
            if (exception != null)
            {
                e = exception.InnerException;
            }
            this.translator.throwError(luaState, e);
        }

        private int toString(IntPtr luaState)
        {
            object obj2 = this.translator.getRawNetObject(luaState, 1);
            if (obj2 != null)
            {
                this.translator.push(luaState, obj2.ToString() + ": " + obj2.GetHashCode());
            }
            else
            {
                LuaDLL.lua_pushnil(luaState);
            }
            return 1;
        }

        private bool trySetMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType, out string detailMessage)
        {
            detailMessage = null;
            if (LuaDLL.lua_type(luaState, 2) != LuaTypes.LUA_TSTRING)
            {
                detailMessage = "property names must be strings";
                return false;
            }
            string memberName = LuaDLL.lua_tostring(luaState, 2);
            if (((memberName == null) || (memberName.Length < 1)) || (!char.IsLetter(memberName[0]) && (memberName[0] != '_')))
            {
                detailMessage = "invalid property name";
                return false;
            }
            MemberInfo member = (MemberInfo) this.checkMemberCache(this.memberCache, targetType, memberName);
            if (member == null)
            {
                MemberInfo[] infoArray = targetType.GetMember(memberName, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
                if (infoArray.Length <= 0)
                {
                    detailMessage = "field or property '" + memberName + "' does not exist";
                    return false;
                }
                member = infoArray[0];
                this.setMemberCache(this.memberCache, targetType, memberName, member);
            }
            if (member.MemberType == MemberTypes.Field)
            {
                FieldInfo info2 = (FieldInfo) member;
                object obj2 = this.translator.getAsType(luaState, 3, info2.FieldType);
                try
                {
                    info2.SetValue(target, obj2);
                }
                catch (Exception exception)
                {
                    this.ThrowError(luaState, exception);
                }
                return true;
            }
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo info3 = (PropertyInfo) member;
                object obj3 = this.translator.getAsType(luaState, 3, info3.PropertyType);
                try
                {
                    info3.SetValue(target, obj3, null);
                }
                catch (Exception exception2)
                {
                    this.ThrowError(luaState, exception2);
                }
                return true;
            }
            detailMessage = "'" + memberName + "' is not a .net field or property";
            return false;
        }
    }
}

