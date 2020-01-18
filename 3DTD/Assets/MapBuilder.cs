using System.Collections.Generic;
using UnityEngine;

public class MapBuilder {
    public static Map Build(HeightMap heightMap, float scale = 1) {
        Map result = new Map(heightMap.Width * 2 - 1, heightMap.Height * 2 - 1, scale);

        for (int x = 0; x < heightMap.Width; x++) {
            for (int y = 0; y < heightMap.Height; y++) {
                result[x * 2, y * 2] = new FlatTile(heightMap[x, y]);

                //corners
                if (x < heightMap.Width - 1 && y < heightMap.Height - 1) {
                    var bl = heightMap[x, y];
                    var br = heightMap[x + 1, y];
                    var tl = heightMap[x, y + 1];
                    var tr = heightMap[x + 1, y + 1];


                    if (bl == br && tr == tl && tr == br) {
                        result[x * 2 + 1, y * 2 + 1] = new FlatTile(bl);
                    }
                    else if (bl == br && tl == tr) {
                        if (bl < tl) {
                            result[x * 2 + 1, y * 2 + 1] = new SlopeTile(bl, tl, 0);
                        }
                        else {
                            result[x * 2 + 1, y * 2 + 1] = new SlopeTile(tl, bl, 2);
                        }
                    }
                    else if (tl == bl && tr == br) {
                        if (tl < tr) {
                            result[x * 2 + 1, y * 2 + 1] = new SlopeTile(tl, tr, 1);
                        }
                        else {
                            result[x * 2 + 1, y * 2 + 1] = new SlopeTile(tr, tl, 3);
                        }
                    }
                    else {
                        result[x * 2 + 1, y * 2 + 1] = new CornerTile(tr,br,bl,tl);
                    }
                }

                //edges
                if (x > 0) {
                    var l = heightMap[x - 1, y];
                    var r = heightMap[x, y];
                    if (l == r) {
                        result[x * 2 - 1, y * 2] = new FlatTile(l);
                    }
                    else {
                        if (l < r) {
                            result[x * 2 - 1, y * 2] = new SlopeTile(l,r,1);
                        }else {
                            result[x * 2 - 1, y * 2] = new SlopeTile(r,l, 3);
                        }
                    }
                }
                if (y > 0) {

                    var t = heightMap[x, y];
                    var b = heightMap[x, y - 1];

                    if (t == b) {
                        result[x * 2, y * 2 - 1] = new FlatTile(t);
                    }
                    else {
                        if (t < b) {
                            result[x * 2, y * 2 - 1] = new SlopeTile(t, b, 2);
                        }
                        else {
                            result[x * 2, y * 2 - 1] = new SlopeTile(b, t, 0);
                        }
                    }

                }
            }
        }

        return result;
    }

}
