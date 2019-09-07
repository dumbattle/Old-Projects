public class Map {
    public float[,] map;

    public float this[int x, int y] { get { return map[x, y]; } set { map[x, y] = value; } }
    public float this[Point2 p] { get { return map[p.x, p.y]; } set { map[p.x, p.y] = value; } }


    public int Width { get { return map.GetLength(0); } }
    public int Height { get { return map.GetLength(1); } }
    public int Length { get { return Width * Height; } }


    public Map(int width, int height) {
        map = new float[width, height];
    }

    public Map(float[,] map) {
        this.map = map;
    }
}
