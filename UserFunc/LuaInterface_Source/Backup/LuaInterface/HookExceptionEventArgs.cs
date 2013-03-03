namespace LuaInterface
{
    using System;

    public class HookExceptionEventArgs : EventArgs
    {
        private readonly System.Exception m_Exception;

        public HookExceptionEventArgs(System.Exception ex)
        {
            this.m_Exception = ex;
        }

        public System.Exception Exception
        {
            get
            {
                return this.m_Exception;
            }
        }
    }
}

