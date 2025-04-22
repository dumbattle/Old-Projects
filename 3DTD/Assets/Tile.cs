using UnityEngine;
using System.Collections.Generic;


public abstract class Tile {
    public float height;
    public bool flat = false;
    public Vector2Int index { get; protected set; }

    public abstract TileMeshData GetMeshData(int x, int y);

    public abstract float GetHeight(float x, float y);
    public float GetHeight(Vector2 pos) { return GetHeight(pos.x, pos.y); }
    public float GetHeight(Vector3 pos) { return GetHeight(pos.x, pos.z); }

}

public class FlatTile : Tile {
    public FlatTile(float height)  {
        this.height = height;
        flat = true;
    }


    public override TileMeshData GetMeshData(int x, int y) {
        TileMeshData result = new TileMeshData(false);

        index = new Vector2Int(x, y);

        result.verts[0] = new Vector3((x + .5f)  , height, (y + .5f)  );
        result.verts[1] = new Vector3((x - .5f)  , height, (y + .5f)  );
        result.verts[2] = new Vector3((x + .5f)  , height, (y - .5f)  );
        result.verts[3] = new Vector3((x - .5f)  , height, (y - .5f)  );

        result.uvs[0] = new Vector2(1, 1);
        result.uvs[1] = new Vector2(1, 1);
        result.uvs[2] = new Vector2(1, 1);
        result.uvs[3] = new Vector2(1, 1);

        result.inds[0] = 0;
        result.inds[1] = 2;
        result.inds[2] = 1;
        result.inds[3] = 3;
        result.inds[4] = 1;
        result.inds[5] = 2;

        return result;
    }

    public override float GetHeight(float x, float y) {
        return height;
    }
}
public class SlopeTile : Tile {
    float low, high;
    int slope;

    public SlopeTile(float lowHeight, float highHeight, int slopeDir){
        low = lowHeight;
        high = highHeight;
        slope = slopeDir;

        height = (low + high) / 2;
    }

    public override TileMeshData GetMeshData(int x, int y) {
        TileMeshData result = new TileMeshData(false);
        index = new Vector2Int(x, y);
        switch (slope) {
            case 0:
                result.verts[0] = new Vector3((x + .5f), high, (y + .5f));
                result.verts[1] = new Vector3((x - .5f), high, (y + .5f));
                result.verts[2] = new Vector3((x + .5f), low, (y - .5f));
                result.verts[3] = new Vector3((x - .5f), low, (y - .5f));
                break;
            case 1:
                result.verts[0] = new Vector3((x + .5f), high, (y + .5f));
                result.verts[1] = new Vector3((x - .5f), low, (y + .5f));
                result.verts[2] = new Vector3((x + .5f), high, (y - .5f));
                result.verts[3] = new Vector3((x - .5f), low, (y - .5f));
                break;
            case 2:
                result.verts[0] = new Vector3((x + .5f), low, (y + .5f));
                result.verts[1] = new Vector3((x - .5f), low, (y + .5f));
                result.verts[2] = new Vector3((x + .5f), high, (y - .5f));
                result.verts[3] = new Vector3((x - .5f), high, (y - .5f));
                break;
            case 3:
                result.verts[0] = new Vector3((x + .5f), low, (y + .5f));
                result.verts[1] = new Vector3((x - .5f), high, (y + .5f));
                result.verts[2] = new Vector3((x + .5f), low, (y - .5f));
                result.verts[3] = new Vector3((x - .5f), high, (y - .5f));
                break;
        }

        result.uvs[0] = new Vector2(.5f, .5f);
        result.uvs[1] = new Vector2(.5f, .5f);
        result.uvs[2] = new Vector2(.5f, .5f);
        result.uvs[3] = new Vector2(.5f, .5f);

        result.inds[0] = 0;
        result.inds[1] = 2;
        result.inds[2] = 1;
        result.inds[3] = 3;
        result.inds[4] = 1;
        result.inds[5] = 2;

        return result;
    }


    public override float GetHeight(float x, float y) {
        switch (slope) {
            case 0:
                return Mathf.Lerp(low, high, y);
            case 1:
                return Mathf.Lerp(low, high, x);
            case 2:
                return Mathf.Lerp(high, low, y);
            case 3:
                return Mathf.Lerp(high, low, x);
        }
        return 0;
    }
}
public class CornerTile : Tile {
    float tr, br, bl, tl;
    bool a;
    bool b;
    bool diagA;

    public CornerTile(float tr, float br, float bl, float tl) {
        this.tr = tr;
        this.br = br;
        this.bl = bl;
        this.tl = tl;
        a = (br == tl && (tr == br || tl == bl));
        b = (tr == bl && (br == tr || tl == bl));

        diagA = (br + tl > tr + bl) && !a || b;

        height = (tr + br + tl + bl) / 4;
    }

    public override TileMeshData GetMeshData(int x, int y) {
        TileMeshData result = new TileMeshData(true);
        index = new Vector2Int(x, y);


        if (diagA) {
            result.verts[0] = new Vector3((x + .5f), tr, (y + .5f));
            result.verts[1] = new Vector3((x - .5f), tl, (y + .5f));
            result.verts[2] = new Vector3((x + .5f), br, (y - .5f));
            result.verts[3] = new Vector3((x - .5f), bl, (y - .5f));

            result.verts[4] = new Vector3((x + .5f), tr, (y + .5f));
            result.verts[5] = new Vector3((x - .5f), bl, (y - .5f));

            result.inds[0] = 1;
            result.inds[1] = 0;
            result.inds[2] = 3;

            result.inds[3] = 2;
            result.inds[4] = 5;
            result.inds[5] = 4;

            if (b) {
                if (bl == tl) {
                    result.uvs[0] = new Vector2(1, 1);
                    result.uvs[1] = new Vector2(1, 1);
                    result.uvs[3] = new Vector2(1, 1);

                    result.uvs[2] = new Vector2(.5f, .5f);
                    result.uvs[4] = new Vector2(.5f, .5f);
                    result.uvs[5] = new Vector2(.5f, .5f);

                }
                else {
                    result.uvs[0] = new Vector2(.5f, .5f);
                    result.uvs[1] = new Vector2(.5f, .5f);
                    result.uvs[3] = new Vector2(.5f, .5f);

                    result.uvs[2] = new Vector2(1, 1);
                    result.uvs[4] = new Vector2(1, 1);
                    result.uvs[5] = new Vector2(1, 1);
                }
            }
            else {
                result.uvs[0] = new Vector2(.5f, .5f);
                result.uvs[1] = new Vector2(.5f, .5f);
                result.uvs[2] = new Vector2(.5f, .5f);
                result.uvs[3] = new Vector2(.5f, .5f);
                result.uvs[4] = new Vector2(.5f, .5f);
                result.uvs[5] = new Vector2(.5f, .5f);
            }
        }
        else {
            result.verts[0] = new Vector3((x + .5f), tr, (y + .5f));
            result.verts[1] = new Vector3((x - .5f), tl, (y + .5f));
            result.verts[2] = new Vector3((x + .5f), br, (y - .5f));
            result.verts[3] = new Vector3((x - .5f), bl, (y - .5f));
            result.verts[4] = new Vector3((x - .5f), tl, (y + .5f));
            result.verts[5] = new Vector3((x + .5f), br, (y - .5f));

            result.inds[0] = 0;
            result.inds[1] = 2;
            result.inds[2] = 1;

            result.inds[3] = 3;
            result.inds[4] = 4;
            result.inds[5] = 5;

            if (a) {
                if (br == tr) {
                    result.uvs[0] = new Vector2(1, 1);
                    result.uvs[1] = new Vector2(1, 1);
                    result.uvs[2] = new Vector2(1, 1);

                    result.uvs[3] = new Vector2(.5f, .5f);
                    result.uvs[4] = new Vector2(.5f, .5f);
                    result.uvs[5] = new Vector2(.5f, .5f);

                }
                else {
                    result.uvs[0] = new Vector2(.5f, .5f);
                    result.uvs[1] = new Vector2(.5f, .5f);
                    result.uvs[2] = new Vector2(.5f, .5f);

                    result.uvs[3] = new Vector2(1, 1);
                    result.uvs[4] = new Vector2(1, 1);
                    result.uvs[5] = new Vector2(1, 1);
                }
            }
            else {
                result.uvs[0] = new Vector2(.5f, .5f);
                result.uvs[1] = new Vector2(.5f, .5f);
                result.uvs[2] = new Vector2(.5f, .5f);
                result.uvs[3] = new Vector2(.5f, .5f);
                result.uvs[4] = new Vector2(.5f, .5f);
                result.uvs[5] = new Vector2(.5f, .5f);
            }
        }

        return result;
    }



    public override float GetHeight(float x, float y) {
        if (diagA) {
        bool top = y > x;
            if (top) {
                if (b && tr == tl) {
                    return tl;
                }
                else {
                    return Mathf.Lerp(Mathf.Lerp(bl, tl, y), Mathf.Lerp(bl, tr, y), x / y);
                }
            }
            else {
                if (b && br == bl) {
                    return bl;
                }
                else {
                    return Mathf.Lerp(Mathf.Lerp(bl, tr, y), Mathf.Lerp(br, tr, y), (x - y) / (1 - y));
                }
            }
        }
        else {
            bool top = y > 1 - x;

            if (top) {
                if (b && tr == tl) {
                    return tl;
                }
                else {
                    return Mathf.Lerp(Mathf.Lerp(tl, br, x), Mathf.Lerp(tl, tr, x), (y - 1 + x) / (x));
                }
            }
            else {
                if (b && br == bl) {
                    return bl;
                }
                else {
                    return Mathf.Lerp(Mathf.Lerp(bl, br, x), Mathf.Lerp(tl, br, x), y / (1 - x));

                }
            }
        }
    }
}

public class TileMeshData {
    public Vector3[] verts;
    public Vector2[] uvs;
    public int[] inds;


    public TileMeshData(bool sepTriangles) {
        if (sepTriangles) {
            verts = new Vector3[6];
            uvs=new  Vector2[6];
        }else {
            verts = new Vector3[4];
            uvs = new Vector2[4];
        }
        inds = new int[6];
    }
}