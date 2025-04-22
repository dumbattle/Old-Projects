using UnityEngine;


public class MapLayout {
    public int width { get; private set; }
    public int height { get; private set; }

    public TileType[,] data { get; private set; }

    public MapLayout(int w, int h) {
        width = w;
        height = h;
        data = new TileType[w, h];
    }
}


