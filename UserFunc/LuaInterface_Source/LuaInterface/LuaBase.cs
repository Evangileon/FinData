namespace LuaInterface
{
    using System;

    public abstract class LuaBase
    {
        private bool _Disposed;
        protected Lua _Interpreter;
        protected int _Reference;

        protected LuaBase()
        {
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposeManagedResources)
        {
            if (!this._Disposed)
            {
                if (disposeManagedResources && (this._Reference != 0))
                {
                    this._Interpreter.dispose(this._Reference);
                }
                this._Disposed = true;
            }
        }

        public override bool Equals(object o)
        {
            if (o is LuaBase)
            {
                LuaBase base2 = (LuaBase) o;
                return this._Interpreter.compareRef(base2._Reference, this._Reference);
            }
            return false;
        }

        ~LuaBase()
        {
            this.Dispose(false);
        }

        public override int GetHashCode()
        {
            return this._Reference;
        }
    }
}

