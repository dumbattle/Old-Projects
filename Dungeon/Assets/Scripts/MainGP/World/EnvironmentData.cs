using LPE.SpacePartition;
using LPE.Triangulation;
using UnityEngine;



namespace MainGP {
    public class EnvironmentData {
        public Partition2D<RectangleShape> walls;
        public Partition2D<RectangleShape> holes;

        public RectangleShape borderU;
        public RectangleShape borderR;
        public RectangleShape borderD;
        public RectangleShape borderL;

        public Delaunay triangulation_WH;
        public Delaunay triangulation_W;
        public Delaunay triangulation_H;

        public EnvironmentData(MapLayout ml) {
            int w = ml.width;
            int h = ml.height;

            walls = new Grid2D<RectangleShape>(new Vector2(0, 0), new Vector2(w, h), new Vector2Int(w, h));
            holes = new Grid2D<RectangleShape>(new Vector2(0, 0), new Vector2(w, h), new Vector2Int(w, h));
            borderU = new RectangleShape(w, 1, new Vector2(w / 2f, h + .5f));
            borderR = new RectangleShape(1, h, new Vector2(w + .5f, h / 2f));
            borderD = new RectangleShape(w, 1, new Vector2(w / 2f, -.5f));
            borderL = new RectangleShape(1, h, new Vector2(-.5f, h / 2f));

            triangulation_WH = new Delaunay();
            triangulation_W = new Delaunay();
            triangulation_H = new Delaunay();

            triangulation_WH.AddConstraints(new[] {
                new Vector2(0,0), new Vector2(0,h),
                new Vector2(0,h), new Vector2(w,h),
                new Vector2(w,h), new Vector2(w,0),
                new Vector2(h,0), new Vector2(0,0)
            });
            triangulation_W.AddConstraints(new[] {
                new Vector2(0,0), new Vector2(0,h),
                new Vector2(0,h), new Vector2(w,h),
                new Vector2(w,h), new Vector2(w,0),
                new Vector2(h,0), new Vector2(0,0)
            });
            triangulation_H.AddConstraints(new[] {
                new Vector2(0,0), new Vector2(0,h),
                new Vector2(0,h), new Vector2(w,h),
                new Vector2(w,h), new Vector2(w,0),
                new Vector2(w,0), new Vector2(0,0)
            });

            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    var t = ml.data[x, y];

                    switch (t) {
                        case TileType.floor:
                            break;
                        case TileType.hole:
                            var r = new RectangleShape(1, 1, new Vector2(x + .5f, y + .5f));
                            holes.Add(r, r.AABB());


                            triangulation_H.AddConstraints(new[] {
                                new Vector2(x, y), new Vector2(x, y + 1),
                                new Vector2(x, y + 1), new Vector2(x+1, y + 1),
                                new Vector2(x+1, y + 1), new Vector2(x+1, y),
                                new Vector2(x+1, y), new Vector2(x, y)
                            });
                            triangulation_WH.AddConstraints(new[] {
                                new Vector2(x, y), new Vector2(x, y + 1),
                                new Vector2(x, y + 1), new Vector2(x+1, y + 1),
                                new Vector2(x+1, y + 1), new Vector2(x+1, y),
                                new Vector2(x+1, y), new Vector2(x, y)
                            });

                            break;
                        case TileType.wall:
                            var r2 = new RectangleShape(1, 1, new Vector2(x + .5f, y + .5f));
                            walls.Add(r2, r2.AABB());
                            triangulation_W.AddConstraints(new[] {
                                new Vector2(x, y), new Vector2(x, y + 1),
                                new Vector2(x, y + 1), new Vector2(x+1, y + 1),
                                new Vector2(x+1, y + 1), new Vector2(x+1, y),
                                new Vector2(x+1, y), new Vector2(x, y)
                            });
                            triangulation_WH.AddConstraints(new[] {
                                new Vector2(x, y), new Vector2(x, y + 1),
                                new Vector2(x, y + 1), new Vector2(x+1, y + 1),
                                new Vector2(x+1, y + 1), new Vector2(x+1, y),
                                new Vector2(x+1, y), new Vector2(x, y)
                            });
                            break;
                    }
                }
            }
        }

        public void DrawGizmos() {
            triangulation_WH.DrawGizmos(Color.green, Color.red);
        }

    }

}
