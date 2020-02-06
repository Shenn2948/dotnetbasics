using System;
using System.Runtime.InteropServices;

namespace FixMe
{
    public class MyClass : IDisposable
    {
        private IntPtr _buffer; // unmanaged resource

        private SafeHandle _resource; // managed resource

        private bool _disposed = false;

        public MyClass()
        {
            _buffer = Helper.AllocateBuffer();
            _resource = Helper.GetResource();
        }

        ~MyClass()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            Marshal.FreeHGlobal(this._buffer);

            if (disposing)
            {
                if (_buffer == null)
                {
                    Helper.DeallocateBuffer(_buffer);
                }

                _resource.Dispose();
            }

            _disposed = true;
        }

        public void DoSomething()
        {
            // Manipulation with _buffer and _resource.
            if (_disposed)
            {
                throw new ObjectDisposedException("This object is already disposed.");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var myClass = new MyClass())
            {
                myClass.DoSomething();
                myClass.Dispose();
                myClass.DoSomething();
            }
        }
    }
}