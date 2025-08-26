using System;
using System.Collections.Generic;

namespace TestCLI
{
    // Java List<T> benzeri generic interface
    public interface IMyList<T>
    {
        void Add(T item);
        T Get(int index);
        void Remove(T item);
        int Size();
        bool Contains(T item);
    }
}
