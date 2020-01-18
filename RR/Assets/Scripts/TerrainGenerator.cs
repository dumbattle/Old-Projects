using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {
    public Vector2Int mapSize;
    public CompoundPerlin perlin;
    public bool smooth;
    Mesh mesh;
    void Start() {
        var map = CreateHeightMap(mapSize);

        mesh = CreateTerrainMesh(map);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

    }

    void Update() {
        //perlin.layers[1].perlin.Offsets += Vector2.one * Time.deltaTime;

        var map = CreateHeightMap(mapSize);
        if (smooth) {
            map = SmoothMap(map);
            //SmoothMap(map);

        }
        SetVertices(mesh, map);

        mesh.RecalculateNormals();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }


    float[,] CreateHeightMap(Vector2Int mapSize) {
        float[,] map = new float[mapSize.x + 1, mapSize.y + 1];

        float offsetX = mapSize.x / 2f;
        float offsetY = mapSize.y / 2f;

        //create map
        for (int x = 0; x < mapSize.x + 1; x++) {
            for (int y = 0; y < mapSize.y + 1; y++) {
                float v = perlin.GetValue(x, y);
                map[x, y] = v ;
            }
        }

        return map;
    }

    public Mesh CreateTerrainMesh(float[,] heightMap) {
        Mesh mesh = new Mesh();

        Vector2Int mapSize = new Vector2Int(heightMap.GetLength(0) - 1, heightMap.GetLength(1) - 1);


        Vector3[] vertices = new Vector3[(mapSize.x + 1) * (mapSize.y + 1)];
        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[mapSize.x * mapSize.y * 2 * 3];

        SetVertices(mesh, heightMap);
        for (int x = 0; x <= mapSize.x; x++) {
            for (int y = 0; y <= mapSize.y; y++) {
                int i = x + y * (mapSize.x + 1);
                uvs[i] = new Vector2(x, y) / (mapSize);
            }
        }
        int count = 0;


        //set triangles and normals
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                count++;
                int start = (x + y * (mapSize.x)) * 6;

                int v1 = x + y * (mapSize.x + 1);
                int v2 = x + (y + 1) * (mapSize.x + 1);
                int v3 = x + 1 + y * (mapSize.x + 1);
                int v4 = x + 1 + (y + 1) * (mapSize.x + 1);
                triangles[start] = v1;
                triangles[start + 1] = v2;
                triangles[start + 2] = v3;

                triangles[start + 3] = v4;
                triangles[start + 4] = v3;
                triangles[start + 5] = v2;
            }
        }

        mesh.triangles = triangles;
        mesh.uv = uvs;

        return mesh;
    }

    static void SetVertices(Mesh mesh, float[,] heightMap) {
        Vector2Int mapSize = new Vector2Int(heightMap.GetLength(0) - 1, heightMap.GetLength(1) - 1);
        Vector3[] vertices = new Vector3[(mapSize.x + 1) * (mapSize.y + 1)];

        float offsetX = mapSize.x / 2f;
        float offsetY = mapSize.y / 2f;

        //set vertices and uvs
        for (int x = 0; x <= mapSize.x; x++) {
            for (int y = 0; y <= mapSize.y; y++) {
                int i = x + y * (mapSize.x + 1);

                vertices[i] =
                    new Vector3(
                        x - offsetX,
                        heightMap[x, y],
                        y - offsetY);
            }
        }
        mesh.vertices = vertices;
    }

    float[,] SmoothMap (float[,] map) {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        float[,] result = new float[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float sum = 0;
                int count = 0;
                for (int dx = -1; dx <= 1; dx++) {
                    for (int dy = -1; dy <= 1; dy++) {
                        if (InRange(x + dx, y + dy)) {
                            count++;
                            sum += map[x + dx, y + dy];
                        }
                    }
                }

                result[x, y] = sum / count;
            }
        }
        return result;
        bool InRange(int x, int y) {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}
