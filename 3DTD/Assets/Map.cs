using UnityEngine;
using System.Collections.Generic;


public class Map : Map2D<Tile> {
    public float scale { get; private set; }
    public float heightScale { get; private set; }
    public Vector2 offset { get; private set; }

    public Map(int w, int h) : this(w, h, 1, 1) { }
    public Map(int w, int h, float scale) : this(w, h, scale, scale) { }
    public Map(int w, int h, float scale, float heightScale) : base(w, h) {
        this.scale = scale;
        this.heightScale = heightScale;
        offset = new Vector2(-w * scale / 2, -h * scale / 2);
    }

    public Mesh CreateMesh() {
        Mesh result = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> indices = new List<int>();

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                var data = this[x, y].GetMeshData(x, y);
                int offset = vertices.Count;


                var verts = data.verts;
                for (int i = 0; i < verts.Length; i++) {
                    verts[i] = new Vector3(verts[i].x * scale + this.offset.x, verts[i].y * heightScale, verts[i].z * scale + this.offset.y);
                }

                vertices.AddRange(data.verts);

                uvs.AddRange(data.uvs);
                foreach (var i in data.inds) {
                    indices.Add(i + offset);
                }
            }
        }

        result.vertices = vertices.ToArray();
        result.triangles = indices.ToArray();
        result.uv = uvs.ToArray();
        result.RecalculateNormals();


        return result;
    }





    /// TODO
    /// adjust for scale and offset

    public float GetHeight(float x, float y) {
        //add .5f for correct rounding
        x = (x - offset.x )/ scale + .5f ;
        y = (y - offset.y) / scale + .5f ;

        var t = this[(int)x, (int)y];

        return t.GetHeight(x % 1, y % 1) * heightScale;
    }
    public float GetHeight(Vector2 position) {
        position -= offset;
        float x = position.x / scale + .5f;
        float y = position.y / scale + .5f;

        var t = this[(int)x, (int)y];

        return t.GetHeight(x % 1, y % 1) * heightScale;
    }
    public float GetHeight(Vector3 position) {
        float x = (position.x - offset.x) / scale + .5f;
        float y = (position.z - offset.y) / scale + .5f;

        var t = this[(int)x, (int)y];

        return t.GetHeight(x % 1, y % 1) * heightScale;
    }

    public Tile GetTile(Vector2 position) {
        return this[(int)((position.x - offset.x )/ scale + .5f), (int)((position.y - offset.y) / scale + .5f )];
    }
    public Tile GetTile(Vector3 position) {
        return this[(int)((position.x - offset.x) / scale + .5f), (int)((position.z - offset.y )/ scale + .5f)];
    }
}

public class Map2D<T> {
    T[][] data;

    public T this[int x, int y] {
        get => data[x][y];
        set => data[x][y] = value;
    }
    public T this[Vector2Int ind] {
        get => data[ind.x][ind.y];
        set => data[ind.x][ind.y] = value;
    }

    public int Width { get; private set; }
    public int Height { get; private set; }

    public Map2D(int w, int h) {
        data = new T[w][];

        for (int i = 0; i < w; i++) {
            data[i] = new T[h];
        }

        Width = w;
        Height = h;
    }

    public bool IsInRange(int x, int y) {
        return x >= 0 && y >= 0 && x < Width && y < Height;
    }
    public bool IsInRange(Vector2Int pos) {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Width && pos.y < Height;
    }
}