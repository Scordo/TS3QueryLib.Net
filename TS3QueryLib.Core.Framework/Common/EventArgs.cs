using System;

namespace TS3QueryLib.Core.Common
{
    public class EventArgs<T> : EventArgs
    {
        public T Value { get; protected set; }

        public EventArgs(T value)
        {
            Value = value;
        }
    }
}