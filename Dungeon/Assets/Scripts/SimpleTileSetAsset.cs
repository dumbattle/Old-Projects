using UnityEngine;

[CreateAssetMenu(menuName = "Tile Set/Simple")]
public class SimpleTileSetAsset : TileSetAsset {
    public Material groundMat;
    public Material waterGridMat;
    public GameObject wallObj;

    public override GameObject GenerateMap(MapLayout ml) {
        GameObject result = new GameObject("Environment");

        BuildFloor(ml, result);
        BuildWater(ml, result);
        BuildWalls(ml, result);
        return result;

    }

    void BuildFloor(MapLayout ml, GameObject result) {
        GameObject floor = new GameObject("Floor");
        floor.transform.parent = result.transform;

        var ff = floor.AddComponent<MeshFilter>();
        var fr = floor.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();

        m.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, ml.height), new Vector3(ml.width, 0, 0), new Vector3(ml.width, 0, ml.height) };
        m.triangles = new int[] { 0, 1, 2, 2, 1, 3 };


        ff.mesh = m;
        fr.material = groundMat;
    }


    void BuildWater(MapLayout ml, GameObject result) {

        GameObject water = new GameObject("Water");
        water.transform.parent = result.transform;

        var ff = water.AddComponent<MeshFilter>();
        var fr = water.AddComponent<MeshRenderer>();

        Mesh m = new Mesh();

        m.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 0, ml.height), new Vector3(ml.width, 0, 0), new Vector3(ml.width, 0, ml.height) };
        m.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        m.uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };


        ff.mesh = m;

        Texture2D waterTex = new Texture2D(ml.width, ml.height);
        waterTex.filterMode = FilterMode.Point;
        for (int x = 0; x < ml.width; x++) {
            for (int y = 0; y < ml.height; y++) {
                if (ml.data[x, y] == TileType.hole) {
                    waterTex.SetPixel(x, y, new Color(1, 1, 1, 1));
                }
                else {
                    waterTex.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }
        waterTex.Apply();
        waterGridMat.SetTexture("_WaterCells", waterTex);
        fr.material = waterGridMat;

    }


    void BuildWalls(MapLayout ml, GameObject result) {
        GameObject walls = new GameObject("Walls");
        walls.transform.parent = result.transform;

        for (int x = 0; x < ml.width; x++) {
            for (int y = 0; y < ml.height; y++) {
                if (ml.data[x, y] == TileType.wall) {
                    var w = Instantiate(wallObj);
                    w.transform.position = new Vector3(x + .5f, 0, y + .5f);
                    w.transform.parent = walls.transform.parent;
                }
            }
        }
    }
}

