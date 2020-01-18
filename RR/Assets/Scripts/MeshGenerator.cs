using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour {
    public Vector2Int mapSize = new Vector2Int(5, 5);
    public Vector2 tileSize = new Vector2(1, 1);
    public float height = 5;
    public Vector4 ld;
    Mesh mesh;

    Vector3[] vertices;
    Vector3[] normals;
    Vector2[] uvs;
    int[] triangles;

    Texture2D t;
    float[,] map;

    void Start() {
        mesh = new Mesh();
        t = new Texture2D(mapSize.x + 1, mapSize.y + 1);
        map = new float[mapSize.x + 1, mapSize.y + 1];

        vertices = new Vector3[(mapSize.x + 1) * (mapSize.y + 1)];
        triangles = new int[mapSize.x * mapSize.y * 2 * 3];
        uvs = new Vector2[vertices.Length];
        normals = new Vector3[vertices.Length];


        float offsetX = mapSize.x * tileSize.x / 2f;
        float offsetY = mapSize.y * tileSize.y / 2f;

        //create map
        for (int x = 0; x < mapSize.x + 1; x++) {
            for (int y = 0; y < mapSize.y + 1; y++) {
                float v = Mathf.PerlinNoise(x / 40f, y / 40f);
                map[x, y] = v;
            }
        }

        //set vertices and uvs
        for (int x = 0; x < mapSize.x + 1; x++) {
            for (int y = 0; y < mapSize.y + 1; y++) {
                int i = x + y * (mapSize.x + 1);

                vertices[i] =
                    new Vector3(
                        x * tileSize.x - offsetX,
                        map[x, y] * height,
                        y * tileSize.y - offsetY);


                uvs[i] = new Vector2(x, y) / (mapSize);
            }
        }
        int count = 0;


        //set triangles and normals
        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                count++;
                float nx, ny, nz;
                Vector3 e1, e2;

                int start = (x + y * (mapSize.x)) * 6;

                int v1 = x + y * (mapSize.x + 1);
                int v2 = x + (y + 1) * (mapSize.x + 1);
                int v3 = x + 1 + y * (mapSize.x + 1);
                int v4 = x + 1 + (y + 1) * (mapSize.x + 1);
                triangles[start] = v1;
                triangles[start + 1] = v2;
                triangles[start + 2] = v3;

                //normal
                e1 = vertices[v1] - vertices[v2];
                e2 = vertices[v1] - vertices[v3];

                nx = e1.y * e2.z - e1.z * e2.y;
                ny = e1.x * e2.z - e1.z * e2.x;
                nz = e1.y * e2.x - e1.x * e2.y;

                normals[v1] -= new Vector3(nx, ny, nz);
                normals[v2] -= new Vector3(nx, ny, nz);
                normals[v3] -= new Vector3(nx, ny, nz);


                //second triangle
                triangles[start + 3] = v4;
                triangles[start + 4] = v3;
                triangles[start + 5] = v2;

                //normal
                e1 = vertices[v2] - vertices[v4];
                e2 = vertices[v2] - vertices[v3];

                nx = e1.y * e2.z - e1.z * e2.y;
                ny = e1.x * e2.z - e1.z * e2.x;
                nz = e1.y * e2.x - e1.x * e2.y;

                normals[v4] -= new Vector3(nx, ny, nz);
                normals[v2] -= new Vector3(nx, ny, nz);
                normals[v3] -= new Vector3(nx, ny, nz);
            }
        }


        //set texture
        for (int x = 0; x < mapSize.x + 1; x++) {
            for (int y = 0; y < mapSize.y + 1; y++) {
                int i = x + y * (mapSize.x + 1);

                normals[i].Normalize();
                t.SetPixel(x, y, new Color(normals[i].x, normals[i].y, normals[i].z));

            }
        }

        t.Apply();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", t);
        GetComponent<MeshRenderer>().material.SetVector("_LightDirection", ld.normalized);
        //GetComponent<MeshRenderer>().material.SetTextureScale("_MainTex", Vector2.one /  mapSize / tileSize);
    }

    
}
