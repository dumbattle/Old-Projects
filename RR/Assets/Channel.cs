using UnityEngine;

public class Channel {
    public RingBuffer<float> data = new RingBuffer<float>(Graph.MAX_HISTORY);

    public Color color = Color.white;

    public bool isActive = false;

    public string name = "Untitled";


    public int Length => data.Size;

    public bool overrideYRange = false;
    public float yMin = 0;
    public float yMax = 1;

    public Channel(Color C) {
        color = C;
    }    
    public Channel() {
        color = new Color(Random.value, Random.value, Random.value);
    }


    public void Feed(float val) {
        data.Add(val);
    }

    public void SetActive() {
        isActive = true;
    }

    public void SetActive(string name) {
        isActive = true;
        this.name = name;
    }

    public void CalculateYRange() {
        float min = data[0];
        float max = data[0];
        for (int i = 0; i < Length; i++) {
            if (data[i] < min) {
                min = data[i];
            }
            if (data[i] > max) {
                max = data[i];
            }
        }

        float range = max - min;
        yMin = min - range * .05f;
        yMax = max + range * .05f;
    }
}

public class RingBuffer<T> {
    T[] data;

    public T this[int index] {
        get {
            index += startIndex;
            index %= MaxSize;
            return data[index];
        }
    }
    public int MaxSize => data.Length;
    public int Size { get; private set; }

    int index = 0;
    int startIndex => (index - Size + MaxSize) % MaxSize;

    public RingBuffer (int maxSize) {
        data = new T[maxSize];
        Size = 0;
    }

    public void Add(T item) {
        data[index] = item;
        index = (index + 1) % MaxSize;
        Size = Size + 1;
        Size = Size > MaxSize ? MaxSize : Size;
    }


}