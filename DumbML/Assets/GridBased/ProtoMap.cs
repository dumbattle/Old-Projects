using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Floor = true.  Wall = false.
/// </summary>
public class ProtoMap {
    public const bool Floor = true, Wall = false;
    public bool this[int x, int y] { get { return map[x, y]; } set { map[x, y] = value; } }
    public bool this[Point2 p] { get { return map[p.x, p.y]; } set { map[p.x, p.y] = value; } }

    public int Width { get { return map.GetLength(0); } }
    public int Height { get { return map.GetLength(1); } }
    public int Length { get { return Width * Height; } }

    public bool IsBuilt { get { return map != null; } }
    public bool edgesAreWalls = true;
    bool[,] map;


    public ProtoMap(int width, int height, bool initializeWithFloors= false) {
        map = new bool[width, height];
        if (initializeWithFloors) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    map[x, y] = true;
                }
            }
        }
    }

    public static ProtoMap CellularAutomata(int width, int height, float fillScale, int smoothCount, int buffer = 0) {
        ProtoMap result = new ProtoMap(width, height);

        result.RandFill(fillScale, buffer);
        for (int i = 0; i < smoothCount; i++) {
            result.Smooth();
        }


        return result;
    }

    public Map ToMap () {
        Map result = new Map(Width, Height);

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                result[x, y] = map[x, y] ? 1 : 0; 
            }
        }
        return result;
    }


    public void RandFill(float fillScale, int buffer = 0) {
        int width = Width;
        int height = Height;

        for (int x = buffer; x < width - buffer; x++) {
            for (int y = buffer; y < height - buffer; y++) {
                if (Random.value < fillScale) {
                    map[x, y] = true;
                }
            }
        }
    }
    public void Smooth(int count = 1) {
        int width = Width;
        int height = Height;

        for (int i = 0; i < count; i++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {

                    int wallCount = GetSurroundingWallCount(x, y);

                    if (wallCount < 4) {
                        map[x, y] = true;
                    }
                    else if (wallCount > 4) {
                        map[x, y] = false;
                    }
                }
            }
        }
    }
    public void ClearSmallerRegions(int numRooms = 1, List<List<Point2>> rooms = null) {
        if (rooms == null) {
            rooms = GetRegions();
        }

        rooms = (from r in rooms orderby r.Count descending select r).ToList();

        for (int i = numRooms; i < rooms.Count; i++) {
            foreach (Point2 p in rooms[i]) {
                map[p.x, p.y] = false;

            }
        }
    }
    public void ClearSmallRegions(int threshold = 50, List<List<Point2>> rooms = null) {
        if (rooms == null) {
            rooms = GetRegions();
        }

        rooms = (from r in rooms orderby r.Count descending select r).ToList();

        for (int i = 1; i < rooms.Count; i++) {
            if (rooms[i].Count < threshold) {
                foreach (Point2 p in rooms[i]) {
                    map[p.x, p.y] = false;

                }
            }
        }
    }

    public float FillAmount() {
        float size = Length;
        float count = 0;
        foreach (var b in map) {
            if (b) count++;
        }
        return count / size;
    }
    public List<List<Point2>> GetRegions() {
        List<List<Point2>> rooms = new List<List<Point2>>();
        bool[,] tilesChecked = new bool[Width, Height];


        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                if (!tilesChecked[x, y] && map[x, y]) {
                    rooms.Add(FloodFill(new Point2(x, y), tilesChecked));
                }
            }
        }


        return rooms;
    }
    public void ExpandFloors(float scale, int count = 1) {
        for (int i = 0; i < count; i++) {

            List<Point2> p = new List<Point2>();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {

                    if (map[x, y] == Wall) {
                        int numFloors = 8 - GetSurroundingWallCount(x, y);
                        if (Random.value < scale * numFloors) {
                            p.Add(new Point2(x, y));

                        }
                    }

                }
            }


            foreach (Point2 point in p) {
                map[point.x, point.y] = Floor;
            }
        }
    }
    public void ExpandWalls(float scale, int count = 1) {
        for (int i = 0; i < count; i++) {

            List<Point2> p = new List<Point2>();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {

                    if (map[x, y] == Floor) {
                        int numWalls =  GetSurroundingWallCount(x, y);
                        if (Random.value < scale * numWalls) {
                            p.Add(new Point2(x, y));
                        }
                    }

                }
            }


            foreach (Point2 point in p) {
                map[point.x, point.y] = Wall;
            }
        }
    }

    List<Point2> FloodFill(Point2 start, bool[,] tilesChecked = null) {
        List<Point2> results = new List<Point2>();
        Queue<Point2> queue = new Queue<Point2>();
        if (tilesChecked == null) {
            tilesChecked = new bool[Width, Height];
        }
        queue.Enqueue(start);
        tilesChecked[start.x, start.y] = true;

        int s = 0;

        while (queue.Count > 0) {
            ++s;
            if (s > Width * Height) {
                break;
            }

            Point2 tile = queue.Dequeue();
            results.Add(tile);

            for (int x = tile.x - 1; x <= tile.x + 1; x++) {
                for (int y = tile.y - 1; y <= tile.y + 1; y++) {

                    if (map.IsInRange(x, y) && (y == tile.y || x == tile.x)) {
                        if (!tilesChecked[x, y] && map[x, y]) {

                            tilesChecked[x, y] = true;
                            queue.Enqueue(new Point2(x, y));
                        }
                    }
                }
            }
        }

        return results;
    }
    int GetSurroundingWallCount(int x, int y) {
        int result = 0;

        for (int i = x - 1; i <= x + 1; i++) {
            for (int j = y - 1; j <= y + 1; j++) {

                //skip self
                if (i == x && j == y)
                    continue;

                if (i < 0 || j < 0 || i == Width || j == Height) {
                    if (edgesAreWalls) {
                        result++;
                    }
                    continue;
                }

                if (!map[i, j]) {
                    result++;
                }

            }
        }

        return result;
    }
}