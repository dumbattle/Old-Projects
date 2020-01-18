using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Tester : MonoBehaviour {
    public Vector2Int mapSize;
    public static Map map;
    Mesh mesh;

    public GameObject mouseIndicator;

    public GameObject unitObj;
    public float scale = 1;

    void Start() {
        unitObj.SetActive(false);
        BuildMesh();
    }

    void Update() {
        IndicateMouse();
    }

    IEnumerator Next() {
        var obj = Instantiate(unitObj, new Vector3(0, map.GetHeight(0, 0), 0), Quaternion.identity);
        obj.SetActive(true);

        while (true) {
            if (InputManager.Click) {
                var click = GetMousePosition();
                if (click != null) {
                    var pos = click.Value;
                    var dir = pos - obj.transform.position;
                    dir.y = 0;
                    dir.Normalize();
                    dir /= 10f;

                    int framecount = 0;
                    while (true) {
                        framecount++;
                        obj.transform.position += dir;

                        obj.transform.position = new Vector3(
                            obj.transform.position.x,
                            map.GetHeight(obj.transform.position),
                            obj.transform.position.z);

                        if ((pos - obj.transform.position).sqrMagnitude < dir.sqrMagnitude) {
                            print($"{framecount} frames");
                            break;
                        }

                        yield return null;
                    }
                }
            }

            yield return null;
        }
    }


    void BuildMesh() {
        var hm = new HeightMap(mapSize.x, mapSize.y);

        hm.AddHill(5, 5, 12, 10, .5f);

        map = MapBuilder.Build(hm,scale);
        mesh = map.CreateMesh();



        var go = new GameObject("Terrain Mesh");

        var mFilter = go.AddComponent<MeshFilter>();
        var mCollider = go.AddComponent<MeshCollider>();
        var mRenderer = go.AddComponent<MeshRenderer>();



        mFilter.sharedMesh = mesh;
        mCollider.sharedMesh = mesh;

        Texture2D tex = new Texture2D(2, 2);
        tex.SetPixel(0, 0, Color.green);
        tex.SetPixel(1, 1, Color.red);
        tex.SetPixel(0, 1, Color.blue);

        tex.filterMode = FilterMode.Point;
        tex.Apply();


        mRenderer.material.SetTexture(mRenderer.material.GetTexturePropertyNames()[0], tex);
        StartCoroutine(Next());
    }


    void IndicateMouse() {
        var hit = GetMousePosition();
        if (hit == null) {
            return;
        }
        var pos = hit.Value;

        mouseIndicator.transform.position = new Vector3(pos.x, map.GetHeight(pos), pos.z);
    }

    Vector3? GetMousePosition() {
        var r = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(r, out RaycastHit hit)) {
            return hit.point;
        }

        return null;
    }
}
