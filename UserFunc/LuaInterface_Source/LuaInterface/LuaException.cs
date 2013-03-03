namespace LuaInterface
{
    using System;

    public class LuaException : ApplicationException
    {
        public LuaException(string reason) : base(reason)
        {
        }
    }
}

