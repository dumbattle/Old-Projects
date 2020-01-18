﻿using System.Collections;
using System.Collections.Generic;


public class RingBuffer<T> : IEnumerable<T> {
    T[] data;

    public T this[int index] {
        get {
            index += startIndex;
            index %= MaxSize;
            return data[index];
        }
    }
    public int MaxSize => data.Length;
    public int Count { get; private set; }

    int nextIndex = 0;
    int startIndex => (nextIndex - Count + MaxSize) % MaxSize;

    public RingBuffer(int maxSize) {
        data = new T[maxSize];
        Count = 0;
    }

    public void Add(T item) {
        data[nextIndex] = item;
        nextIndex = (nextIndex + 1) % MaxSize;
        Count = Count + 1;
        Count = Count > MaxSize ? MaxSize : Count;
    }

    public void Reset() {
        nextIndex = 0;
        Count = 0;
    }


    public IEnumerator<T> GetEnumerator() {
        for (int i = 0; i < Count; i++) {
            yield return this[i];
        }
    }
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}