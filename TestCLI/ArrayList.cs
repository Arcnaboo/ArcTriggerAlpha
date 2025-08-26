using System;

namespace TestCLI
{
    public class ArrayList<T> : IMyList<T>
    {
        private T[] array;
        private int count; // eleman sayısı

        public ArrayList(int capacity = 10)
        {
            array = new T[capacity];
            count = 0;
        }

        public void Add(T item)
        {
            if (count == array.Length)
            {
                // Kapasite dolduğunda büyüt
                Resize();
            }
            array[count++] = item;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (Equals(array[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();
            return array[index];
        }

        public void Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (Equals(array[i], item))
                {
                    // Elemanı bulduk → sonraki elemanları kaydır
                    for (int j = i; j < count - 1; j++)
                    {
                        array[j] = array[j + 1];
                    }
                    array[count - 1] = default!; // sonuncuyu temizle
                    count--;
                    return;
                }
            }
        }

        public int Size()
        {
            return count;
        }

        private void Resize()
        {
            int newCapacity = array.Length * 2;
            T[] newArray = new T[newCapacity];
            Array.Copy(array, newArray, count);
            array = newArray;
        }
    }
}
