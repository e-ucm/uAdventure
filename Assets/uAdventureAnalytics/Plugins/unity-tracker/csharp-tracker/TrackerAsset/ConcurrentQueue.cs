using System;
using System.Collections;
using System.Collections.Generic;

namespace AssetPackage {
	
	public class ConcurrentQueue<T>{
		
		private readonly object syncLock = new object();
		private LinkedList<T> queue;

		public int Count
		{
			get
			{
				lock(syncLock) 
				{
					return queue.Count;
				}
			}
		}

		public ConcurrentQueue()
		{
			this.queue = new LinkedList<T>();
		}

		public T[] Peek(UInt32 n = 1)
		{
			lock(syncLock)
			{
                n = System.Math.Min((UInt32)queue.Count, n);

                T[] tmp = new T[n];

                LinkedListNode<T> it = queue.First;
                for(UInt32 i = 0; i < n; i++)
                {
                    tmp[i] = it.Value;
                    it = it.Next;
                }

				return tmp;
			}
		}

        public void Enqueue(T obj)
		{
			lock(syncLock)
			{
				queue.AddLast(obj);
			}
		}

        public T Dequeue()
        {
			lock(syncLock)
			{
                T tmp = queue.First.Value;
                queue.RemoveFirst();
                return tmp;
        	}
		}

        public void Dequeue(int n)
        {
            lock (syncLock)
            {
                for (int i = 0; i < n; i++)
                {
                    queue.RemoveFirst();
                }
            }
        }


        public void Clear()
		{
			lock(syncLock)
			{
				queue.Clear();
			}
		}

		public T[] CopyToArray()
		{
			lock(syncLock)
			{
				if(queue.Count == 0)
				{
					return new T[0];
				}

				T[] values = new T[queue.Count];
				queue.CopyTo(values, 0);	
				return values;
			}
		}

		public static ConcurrentQueue<T> InitFromArray(IEnumerable<T> initValues)
		{
			var queue = new ConcurrentQueue<T>();

			if(initValues == null)	
			{
				return queue;
			}

			foreach(T val in initValues)
			{
				queue.Enqueue(val);
			}

			return queue;
		}
	}
}