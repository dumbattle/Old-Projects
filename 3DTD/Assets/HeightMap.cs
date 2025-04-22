using UnityEngine;

public class HeightMap {
    int[][] map;

    public int this[int x, int y] {
        get => map[x][y];
        set => map[x][y] = value;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public HeightMap(int width, int height) {
        map = new int[width][];
        for (int i = 0; i < width; i++) {
            map[i] = new int[height];
        }
        Width = width;
        Height = height;
    }

    public void Randomize() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                map[x][y] = Random.Range(0, 3);
            }
        }
    }

    public void AddHill(int x, int y, int height, int radius, float noise) {
        var c = new Circle(new Vector2Int(x, y), radius);

        foreach (var v in c) {
            if (!IsInRange(v.x, v.y)) {
                continue;
            }
            var dist = 1 - new Vector2(v.x - x, v.y - y).magnitude / radius;
            map[v.x][v.y] =(int)( height * dist + UnityEngine.Random.Range(-noise,noise));
        }
    }

    public  bool IsInRange(int x, int y) {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
}
