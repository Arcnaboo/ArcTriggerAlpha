using System;

namespace TestCLI
{
    public class LinkList<T> : IMyList<T>
    {
        private class Node
        {
            public T Value;
            public Node? Next;

            public Node(T value)
            {
                Value = value;
                Next = null;
            }
        }

        private Node? head;
        private Node? tail;
        private int count;

        public LinkList()
        {
            head = null;
            tail = null;
            count = 0;
        }

        public void Add(T item)
        {
            Node newNode = new Node(item);

            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail!.Next = newNode;
                tail = newNode;
            }
            count++;
        }

        public bool Contains(T item)
        {
            Node? current = head;
            while (current != null)
            {
                if (Equals(current.Value, item))
                {
                    return true;
                }
                current = current.Next;
            }
            return false;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException();

            Node? current = head;
            for (int i = 0; i < index; i++)
            {
                current = current!.Next;
            }
            return current!.Value;
        }

        public void Remove(T item)
        {
            Node? current = head;
            Node? previous = null;

            while (current != null)
            {
                if (Equals(current.Value, item))
                {
                    if (previous == null) // baştaki eleman
                    {
                        head = current.Next;
                        if (head == null)
                            tail = null;
                    }
                    else
                    {
                        previous.Next = current.Next;
                        if (current.Next == null) // sondaki eleman
                            tail = previous;
                    }

                    count--;
                    return;
                }

                previous = current;
                current = current.Next;
            }
        }

        public int Size()
        {
            return count;
        }
    }
}
