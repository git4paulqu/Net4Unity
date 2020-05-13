using System;
using System.Collections.Generic;

namespace Net
{
    public sealed class BufferObjectCache
    {
        public static T Alloc<T>(int capacity) where T : BufferObject, new()
        {
            string key = typeof(T).ToString();

            BufferObject bufferObject = GetCacheObject(key);
            if (null != bufferObject)
            {
                bufferObject.OnAlloc();
                return bufferObject as T;
            }

            T allocObject = new T();
            allocObject.Alloc(capacity);
            allocObject.OnAlloc();

            return allocObject;
        }

        public static void Recycle(BufferObject bufferObject)
        {
            string key = bufferObject.GetType().ToString();
            Stack<BufferObject> stack = GetCacheStack(key);

            lock (lockObject)
            {
                bufferObject.OnRecycle();
                stack.Push(bufferObject);
            }
        }

        public static void Dispose()
        {
            lock (lockObject)
            {
                foreach (var item in _map_type2Object)
                {
                    Stack<BufferObject> bufferObjects = item.Value;

                    foreach (var bo in bufferObjects)
                    {
                        bo.Dispose();
                    }
                    bufferObjects.Clear();
                }
                _map_type2Object.Clear();
            }
        }

        private static BufferObject GetCacheObject(string key)
        {
            Stack<BufferObject> stack = GetCacheStack(key);

            lock (lockObject)
            {
                if (stack.Count > 0)
                {
                    return stack.Pop();
                }
                return null;
            }
        }

        private static Stack<BufferObject> GetCacheStack(string key)
        {
            lock (lockObject)
            {
                Stack<BufferObject> cache;
                if (!_map_type2Object.TryGetValue(key, out cache))
                {
                    cache = new Stack<BufferObject>();
                    _map_type2Object.Add(key, cache);
                }
                return cache;
            }
        }

        private static object lockObject = new object();

        private static Dictionary<string, Stack<BufferObject>> _map_type2Object = new Dictionary<string, Stack<BufferObject>>();
    }
}