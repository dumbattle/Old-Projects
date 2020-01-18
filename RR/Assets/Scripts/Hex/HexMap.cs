using System.Collections;
using System.Collections.Generic;


public class HexMap<T> : IEnumerable<T> {
    public HexGrid grid { get; private set; }
    T[] value;

    public T this[HexCoord h] {
        get {
            int i = h.MapIndex();
            return value[i];
        }
        set {
            int i = h.MapIndex();
            this.value[i] = value;
        }
    }

    public int Size => grid.area;
    public int Radius { get; private set; }

    public HexMap(int radius) {
        Radius = radius;
        grid = new HexGrid(radius);
        value = new T[grid.area];
    }

    public IEnumerator<T> GetEnumerator() {
        return ((IEnumerable<T>)value).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator() {
        return ((IEnumerable<T>)value).GetEnumerator();
    }

    public bool IsInRange(HexCoord hex) {
        return hex.ManhattanDist() <= Radius;
    }
}
