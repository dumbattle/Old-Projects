using UnityEngine;

public class Grid<T> {
    T[][] _data;

    public T this[int x, int y] {
        get { return _data[x][y]; }
        set { _data[x][y] = value; }
    }
    public T this[Vector2Int index] {
        get { return _data[index.x][index.y]; }
        set { _data[index.x][index.y] = value; }
    }

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Vector2Int Size { get => new Vector2Int(Width, Height); }

    public Grid(int x, int y) {
        _data = new T[x][];

        for (int i = 0; i < x; i++) {
            _data[i] = new T[y];
        }
        Width = x;
        Height = y;
    }
    public Grid(Vector2Int size) : this(size.x, size.y) { }



    public bool IsInRange(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
    public bool IsInRange(Vector2Int index) {
        return index.x >= 0 && index.x < Width && index.y >= 0 && index.y < Height;
    }
}
