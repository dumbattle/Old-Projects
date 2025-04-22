using System.Collections.Generic;
using System;
using UnityEngine;

public class HexTester : MonoBehaviour {
    public static HexTester current;
    public int mapRadius;
    public int loadRadius = 1;
    public GameObject hexTile;
    public CompoundPerlin perlin;
    public Orientation orientation;

    HexCoord mapCenter;
    HexMap<HexTile> tiles;
    Mesh normalMesh;

    Queue<TileUpdate> updates = new Queue<TileUpdate>();
    private void Awake() {
        current = this;
    }
    private void Start() {
        tiles = new HexMap<HexTile>(loadRadius);
        foreach (var hex in new HexGrid(loadRadius)) {
            var tile = Instantiate(hexTile);
            tile.SetActive(true);
            tiles[hex] = new HexTile(tile, hex);
        }
        mapCenter = (0, 0);

        perlin.GetValue(0, 0);
        SetTiles();

    }
    private void Update() {
        if (updates.Count > 0) {
            var up = updates.Dequeue();
            up.Update();
        }
    }


    public void MoveMap (HexCoord direction) {
        HexMap<HexTile> newMap = new HexMap<HexTile>(loadRadius);
        HexMap<bool> used = new HexMap<bool>(loadRadius);

        int unusedCount = 0;
        mapCenter += direction;

        foreach (var h in new HexGrid(loadRadius)) {
            var newInd = (tiles[h].index) - mapCenter;

            if (newInd.ManhattanDist() <= loadRadius) {
                newMap[newInd] = tiles[h];
                used[h] = true;
            }
            else {
                used[h] = false;
                unusedCount++;
            }
        }


        HexTile[] unused = new HexTile[unusedCount];
        unusedCount = 0;
        foreach (var h in new HexGrid(loadRadius)) {
            if (!used[h]) {
                unused[unusedCount] = tiles[h];
                unusedCount++;
            }
        }

        unusedCount = 0;
        foreach (var h in new HexGrid(loadRadius)) {
            if (newMap[h] == null ) {
                newMap[h] = new HexTile(unused[unusedCount], mapCenter + h);


                unusedCount++;


                Mesh mesh = new Mesh();
                mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                updates.Enqueue(new TileUpdate(newMap[h], mesh));
            }
        }
        tiles = newMap;
        //SetTiles();
    }

    void SetTiles() {

        foreach (var h in new HexGrid(loadRadius)) {
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            HexTile tile = tiles[h];

            updates.Enqueue(new TileUpdate(tile, mesh));
        };
    }
    HexMap<float> CreateHeightMap(int radius, Vector2 center) {
        HexGrid g = new HexGrid(radius);
        HexMap<float> map = new HexMap<float>(radius);

        foreach (var hex in g) {
            var vec = hex.ToCartesian(orientation) + center;
            float v = perlin.GetValue(vec.x, vec.y);

            map[hex] = (int)v;
        }
        return map;
    }

    public (Vector3[], int[]) CreateTerrainMesha(HexMap<float> heightMap) {

        int radius = heightMap.Radius;
        int[] triangles = new int[(heightMap.Size - 1 + (radius * (radius - 1) * 3)) * 3];
        Vector3[] vertices = new Vector3[(heightMap.Size - 1 + (radius * (radius - 1) * 3)) * 9];

        Vector2 cart;
        int ind = 0;

        bool first = true;
        foreach (var hex in new HexGrid(heightMap.Radius)) {
            if (first) {
                first = false;
                continue;
            }
            int arm = hex.Arm;
            var hex2 = hex + HexCoord.Directions[(arm + 0) % 6];
            var hex3 = hex2 + HexCoord.Directions[(arm + 2) % 6];

            int ind1 = hex.MapIndex();
            int ind2 = hex2.MapIndex();
            int ind3 = hex3.MapIndex();

            triangles[ind] = ind;
            cart = hex.ToCartesian(orientation);
            vertices[ind] = new Vector3(cart.x, heightMap[hex], cart.y);
            ind++;

            triangles[ind] = ind;
            cart = hex2.ToCartesian(orientation);
            vertices[ind] = new Vector3(cart.x, heightMap[hex2], cart.y);
            ind++;

            triangles[ind] = ind;
            cart = hex3.ToCartesian(orientation);
            vertices[ind] = new Vector3(cart.x, heightMap[hex3], cart.y);
            ind++;

            var intint = (ind1 - new HexGrid(hex.ManhattanDist() - 1).area) % hex.ManhattanDist();
            if (intint != 0) {
                var hex4 = hex + HexCoord.Directions[(arm + 2) % 6];
                int ind4 = hex4.MapIndex();

                triangles[ind] = ind;
                cart = hex.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex], cart.y);
                ind++;

                triangles[ind] = ind;
                cart = hex3.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex3], cart.y);
                ind++;

                triangles[ind] = ind;
                cart = hex4.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex4], cart.y);
                ind++;
            }
        }
        return (vertices, triangles);
    }
    public Orientation OppositeOrientation (){
        if (orientation == Orientation.horizontal) {
            return Orientation.vertical;
        }
        return Orientation.horizontal;
    }

    class TileUpdate {
        HexTile ht;
        Mesh mesh;
        Orientation orientation => current.orientation;
        Orientation oppositeOrientation => current.OppositeOrientation();
        int radius => current.mapRadius;
        CompoundPerlin perlin => current.perlin;


        public TileUpdate(HexTile ht, Mesh mesh) {
            this.ht = ht;
            this.mesh = mesh;
        }
        public void Update() {
            HexCoord position = ht.index * current. mapRadius;
            Vector2 center = position.ToCartesian(current.OppositeOrientation(), HexCoord.ROOT3);

            var map = CreateHeightMap(center);
            var meshData = CreateTerrainMesh(map);

            mesh.vertices = meshData.v;
            mesh.triangles= meshData.t;
            mesh.RecalculateNormals();

            ht.tile.transform.position = new Vector3(center.x , 0, center.y );
            ht.filter.mesh = mesh;
            ht.collider.sharedMesh = mesh;
        }

        HexMap<float> CreateHeightMap( Vector2 center) {
            HexGrid g = new HexGrid(radius);
            HexMap<float> map = new HexMap<float>(radius);

            foreach (var hex in g) {
                var vec = hex.ToCartesian(orientation) + center;
                float v = perlin.GetValue(vec.x, vec.y);

                map[hex] = (int)v;
            }
            return map;
        }

        public (Vector3[] v, int[] t) CreateTerrainMesh(HexMap<float> heightMap) {

            int radius = heightMap.Radius;
            int[] triangles = new int[(heightMap.Size - 1 + (radius * (radius - 1) * 3)) * 3];
            Vector3[] vertices = new Vector3[(heightMap.Size - 1 + (radius * (radius - 1) * 3)) * 9];

            Vector2 cart;
            int ind = 0;

            bool first = true;
            foreach (var hex in new HexGrid(heightMap.Radius)) {
                if (first) {
                    first = false;
                    continue;
                }
                int arm = hex.Arm;
                var hex2 = hex + HexCoord.Directions[(arm + 0) % 6];
                var hex3 = hex2 + HexCoord.Directions[(arm + 2) % 6];

                int ind1 = hex.MapIndex();
                int ind2 = hex2.MapIndex();
                int ind3 = hex3.MapIndex();

                triangles[ind] = ind;
                cart = hex.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex], cart.y);
                ind++;

                triangles[ind] = ind;
                cart = hex2.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex2], cart.y);
                ind++;

                triangles[ind] = ind;
                cart = hex3.ToCartesian(orientation);
                vertices[ind] = new Vector3(cart.x, heightMap[hex3], cart.y);
                ind++;

                var intint = (ind1 - new HexGrid(hex.ManhattanDist() - 1).area) % hex.ManhattanDist();
                if (intint != 0) {
                    var hex4 = hex + HexCoord.Directions[(arm + 2) % 6];
                    int ind4 = hex4.MapIndex();

                    triangles[ind] = ind;
                    cart = hex.ToCartesian(orientation);
                    vertices[ind] = new Vector3(cart.x, heightMap[hex], cart.y);
                    ind++;

                    triangles[ind] = ind;
                    cart = hex3.ToCartesian(orientation);
                    vertices[ind] = new Vector3(cart.x, heightMap[hex3], cart.y);
                    ind++;

                    triangles[ind] = ind;
                    cart = hex4.ToCartesian(orientation);
                    vertices[ind] = new Vector3(cart.x, heightMap[hex4], cart.y);
                    ind++;
                }
            }
            return (vertices, triangles);
        }
    }
    class HexTile {
        public GameObject tile;
        public MeshFilter filter;
        public MeshCollider collider;
        public HexCoord index;

        public HexTile (GameObject tile, HexCoord index) {
            this.tile = tile;
            this.index = index;
            filter = tile.GetComponent<MeshFilter>();
            collider = tile.GetComponent<MeshCollider>();
        }
        public HexTile(HexTile oldTile, HexCoord newIndex) {
            tile = oldTile.tile;
            index = newIndex;
            filter = oldTile.filter;
            collider = oldTile.collider;
        }
    }
}
