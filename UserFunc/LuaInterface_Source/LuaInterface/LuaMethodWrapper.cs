namespace LuaInterface
{
    using LuaInterface;
    using System;
    using System.Reflection;

    internal class LuaMethodWrapper
    {
        private BindingFlags _BindingType;
        private ExtractValue _ExtractTarget;
        private MethodCache _LastCalledMethod;
        private MemberInfo[] _Members;
        private MethodBase _Method;
        private string _MethodName;
        private object _Target;
        private IReflect _TargetType;
        private ObjectTranslator _Translator;

        public LuaMethodWrapper(ObjectTranslator translator, object target, IReflect targetType, MethodBase method)
        {
            this._LastCalledMethod = new MethodCache();
            this._Translator = translator;
            this._Target = target;
            this._TargetType = targetType;
            if (targetType != null)
            {
                this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
            }
            this._Method = method;
            this._MethodName = method.Name;
            if (method.IsStatic)
            {
                this._BindingType = BindingFlags.Static;
            }
            else
            {
                this._BindingType = BindingFlags.Instance;
            }
        }

        public LuaMethodWrapper(ObjectTranslator translator, IReflect targetType, string methodName, BindingFlags bindingType)
        {
            this._LastCalledMethod = new MethodCache();
            this._Translator = translator;
            this._MethodName = methodName;
            this._TargetType = targetType;
            if (targetType != null)
            {
                this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
            }
            this._BindingType = bindingType;
            this._Members = targetType.UnderlyingSystemType.GetMember(methodName, MemberTypes.Method, (bindingType | BindingFlags.Public) | BindingFlags.IgnoreCase);
        }

        public int call(IntPtr luaState)
        {
            MethodBase method = this._Method;
            object obj2 = this._Target;
            bool flag = true;
            int num = 0;
            if (!(LuaJIT.lua_checkstack(luaState, 5) == 0 ? false : true))
            {
                throw new LuaException("Lua stack overflow");
            }
            bool flag2 = (this._BindingType & BindingFlags.Static) == BindingFlags.Static;
            this.SetPendingException(null);
            if (method != null)
            {
                if ((!method.IsStatic && !method.IsConstructor) && (obj2 == null))
                {
                    obj2 = this._ExtractTarget(luaState, 1);
                    LuaJIT.lua_remove(luaState, 1);
                }
                if (!this._Translator.matchParameters(luaState, method, ref this._LastCalledMethod))
                {
                    this._Translator.throwError(luaState, "invalid arguments to method call");
                    LuaJIT.lua_pushnil(luaState);
                    return 1;
                }
            }
            else
            {
                if (flag2)
                {
                    obj2 = null;
                }
                else
                {
                    obj2 = this._ExtractTarget(luaState, 1);
                }
                if (this._LastCalledMethod.cachedMethod != null)
                {
                    int num2 = flag2 ? 0 : 1;
                    int num3 = LuaJIT.lua_gettop(luaState) - num2;
                    if (num3 == this._LastCalledMethod.argTypes.Length)
                    {
                        if (!(LuaJIT.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6) == 0 ? false : true))
                        {
                            throw new LuaException("Lua stack overflow");
                        }
                        try
                        {
                            for (int j = 0; j < this._LastCalledMethod.argTypes.Length; j++)
                            {
                                this._LastCalledMethod.args[this._LastCalledMethod.argTypes[j].index] = this._LastCalledMethod.argTypes[j].extractValue(luaState, (j + 1) + num2);
                                if ((this._LastCalledMethod.args[this._LastCalledMethod.argTypes[j].index] == null) && !LuaJIT.lua_isnil(luaState, (j + 1) + num2))
                                {
                                    throw new LuaException("argument number " + (j + 1) + " is invalid");
                                }
                            }
                            if ((this._BindingType & BindingFlags.Static) == BindingFlags.Static)
                            {
                                this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(null, this._LastCalledMethod.args));
                            }
                            else if (this._LastCalledMethod.cachedMethod.IsConstructor)
                            {
                                this._Translator.push(luaState, ((ConstructorInfo) this._LastCalledMethod.cachedMethod).Invoke(this._LastCalledMethod.args));
                            }
                            else
                            {
                                this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(obj2, this._LastCalledMethod.args));
                            }
                            flag = false;
                        }
                        catch (TargetInvocationException exception)
                        {
                            return this.SetPendingException(exception.GetBaseException());
                        }
                        catch (Exception exception2)
                        {
                            if (this._Members.Length == 1)
                            {
                                return this.SetPendingException(exception2);
                            }
                        }
                    }
                }
                if (flag)
                {
                    if (!flag2)
                    {
                        if (obj2 == null)
                        {
                            this._Translator.throwError(luaState, string.Format("instance method '{0}' requires a non null target object", this._MethodName));
                            LuaJIT.lua_pushnil(luaState);
                            return 1;
                        }
                        LuaJIT.lua_remove(luaState, 1);
                    }
                    bool flag3 = false;
                    string str = null;
                    foreach (MemberInfo info in this._Members)
                    {
                        str = info.ReflectedType.Name + "." + info.Name;
                        MethodBase base3 = (MethodInfo) info;
                        if (this._Translator.matchParameters(luaState, base3, ref this._LastCalledMethod))
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3)
                    {
                        string e = (str == null) ? "invalid arguments to method call" : ("invalid arguments to method: " + str);
                        this._Translator.throwError(luaState, e);
                        LuaJIT.lua_pushnil(luaState);
                        return 1;
                    }
                }
            }
            if (flag)
            {
                if (!(LuaJIT.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6) == 0 ? false : true))
                {
                    throw new LuaException("Lua stack overflow");
                }
                try
                {
                    if (flag2)
                    {
                        this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(null, this._LastCalledMethod.args));
                    }
                    else if (this._LastCalledMethod.cachedMethod.IsConstructor)
                    {
                        this._Translator.push(luaState, ((ConstructorInfo) this._LastCalledMethod.cachedMethod).Invoke(this._LastCalledMethod.args));
                    }
                    else
                    {
                        this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(obj2, this._LastCalledMethod.args));
                    }
                }
                catch (TargetInvocationException exception3)
                {
                    return this.SetPendingException(exception3.GetBaseException());
                }
                catch (Exception exception4)
                {
                    return this.SetPendingException(exception4);
                }
            }
            for (int i = 0; i < this._LastCalledMethod.outList.Length; i++)
            {
                num++;
                this._Translator.push(luaState, this._LastCalledMethod.args[this._LastCalledMethod.outList[i]]);
            }
            if (num >= 1)
            {
                return num;
            }
            return 1;
        }

        private int SetPendingException(Exception e)
        {
            return this._Translator.interpreter.SetPendingException(e);
        }
    }
}

