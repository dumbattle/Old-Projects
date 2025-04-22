using System.Collections.Generic;
using System;

public class ObjectPool<T> where T : IPoolable {
    List<T> pool;
    bool growable;
    Func<T> constructor;

    public ObjectPool(int capacity,  Func<T> constructor, bool growable = true) {
        pool = new List<T>();
        this.constructor = constructor;
        for (int i = 0; i < capacity; i++) {
            var t = constructor();
            t.Active = false;
            pool.Add(t);
        }
        this.growable = growable;
    }

    public T Get() {
        foreach (var t in pool) {
            if (!t.Active) {
                t.Active = true;
                return t;
            }
        }
        if (growable) {
            var t = constructor();
            pool.Add(t);
            t.Active = true;
            return t;
        }
        UnityEngine.Debug.LogWarning("Object pool has run out of objects");
        return default(T);
    }

    public void Return(T obj) {
        obj.Active = false;
    }
}

public interface IPoolable {
    bool Active { get; set; }
}